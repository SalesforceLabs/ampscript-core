// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Sage.Engine.Transpiler;

/// <summary>
/// Visits AMPscript statements, which are standalone units of execution and do not return values.
/// These are like SET and VAR declarations, and anything that the parser rolls up to ampStatement.
/// </summary>
internal class StatementVisitor : SageParserBaseVisitor<IEnumerable<StatementSyntax>>
{
    private readonly CSharpTranspiler _transpiler;

    public StatementVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    /// <summary>
    /// Generates the C# code for AMPscript IF statements.
    /// </summary>
    /// <remarks>
    /// There is a small disconnect between the AMPscript grammar and the C# grammar, which makes this implementation awkward.
    /// Maybe some day, this will be fixed.
    ///
    /// The AMPscript grammar is of the following form, where elseIf is its own parser rule:
    /// If expression Then contentBlock elseIfStatement* elseStatement? Endif
    ///
    /// C#, on the other hand, has the expression such as:
    /// If expression statement else statement
    ///
    /// Notice how in C#, there is no "else if", because the "if" is considered a single statement after the else.
    /// It's equivalent to:
    /// if (expression) {} else { if (expression) {} }
    /// </remarks>
    public override IEnumerable<StatementSyntax> VisitIfStatement(SageParser.IfStatementContext context)
    {
        var ifStack = new Stack<IfStatementSyntax>();

        ifStack.Push(_transpiler.IfVisitor.Visit(context).WithLineDirective(context.If().Symbol, context.Then().Symbol, _transpiler.SourceFileName));

        foreach (SageParser.ElseIfStatementContext? elseIfExpression in context.elseIfStatement())
        {
            ifStack.Push(_transpiler.IfVisitor.Visit(elseIfExpression));
        }

        ElseClauseSyntax? elseClause = null;
        if (context.elseStatement() != null)
        {
            elseClause =
                ElseClause(TranspilerExtensions.BlockWithHiddenDirectives(_transpiler.BlockVisitor.Visit(context.elseStatement())));
        }

        IfStatementSyntax root = ifStack.Peek();
        while (ifStack.Any())
        {
            root = ifStack.Pop();
            root = root.WithElse(elseClause);
            elseClause = ElseClause(root);
        }

        return new[] { root };
    }

    /// <summary>
    /// Generates the C# code for AMPscript FOR statements.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitForLoop(SageParser.ForLoopContext context)
    {
        /*
         * Generates the following:

            // FOR @COUNTER = 1 TO 5 DO
            // NEXT @COUNTER

            __variables["@counter"] = {assignmentExpression};
            __variables[$"__for_target_@counter_{line}"]  = {targetExpression};
            while (SageValue.ToLong(__variables["@counter"]) <= SageValue.ToLong(__variables[$"__for_target_@counter_{line}"]))
            {
                // ...
            }
         */
        ExpressionSyntax assignmentExpression = _transpiler
            .ExpressionVisitor
            .Visit(context.variableAssignment());

        ExpressionSyntax targetExpression = _transpiler
            .ExpressionVisitor
            .Visit(context.expression());

        var results = new List<StatementSyntax>();

        // FOR @var = {assignExpression} TO {targetExpression} DO
        // NEXT @var

        string assignmentVariableName = context.variableAssignment().VarName().GetText();
        // this resolves the @var = SageValue.ToLong({expression})
        results.Add(_transpiler.Runtime
            .AssignToRuntime(assignmentVariableName, TranspilerExtensions.GetLongValue(assignmentExpression))
            .WithLineDirective(context.variableAssignment(), _transpiler.SourceFileName));

        // This creates a C# variable named "__for_target_{}_{}" and assigns it to SageValue.ToLong({expression})
        // The {variableName} is used in conjunction with the {context.Start.Line} to generate a unique variable name for this loop,
        // because there can be no other loop on this line of the file.  The variable name is included for easier debugging.
        string targetVariableName = $"__for_target_{assignmentVariableName}_{context.Start.Line}";
        results.Add(_transpiler.Runtime
            .AssignToRuntime(targetVariableName, TranspilerExtensions.GetLongValue(targetExpression))
            .WithLineDirective(context.expression(), _transpiler.SourceFileName));

        // This is all of the code inside the DO statement
        // TODO: Insert bound checks
        var statements = new List<StatementSyntax>();
        IEnumerable<StatementSyntax> blocks = _transpiler.BlockVisitor.Visit(context.contentBlock()).ToList();
        if (blocks.Any())
        {
            statements.AddRange(blocks);
        }

        // This controls the (TO|DOWNTO)
        SyntaxKind kindIncrement = SyntaxKind.AddExpression;
        SyntaxKind kindCheck = SyntaxKind.LessThanOrEqualExpression;
        if (context.Downto() != null)
        {
            kindIncrement = SyntaxKind.SubtractExpression;
            kindCheck = SyntaxKind.GreaterThanOrEqualExpression;
        }

        // This adds the counter increment for C#. It looks like the following:
        // __variables["@var"] = ((long)__variables["@var"]) + 1; // (or -1, if it's going down)
        statements
            .Add(_transpiler.Runtime.AssignToRuntime(
                    assignmentVariableName,
                    BinaryExpression(
                        kindIncrement,
                        ParenthesizedExpression(CastExpression(PredefinedType(Token(SyntaxKind.LongKeyword)),
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                _transpiler.Runtime.GetFromRuntime(assignmentVariableName),
                                IdentifierName("Value")))),
                        LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            Literal(1))))
                .WithLineDirective(context.Next(), _transpiler.SourceFileName));

        // Break if we hit the bounds.
        // Without this check the loop would run forever because the long over/under flows to a negative number
        // This is backwards-incompatible with Marketing Cloud because MC returns a "Convergence" error - a type of
        // error reserved for the for loop not completing.
        if (context.To() != null)
        {
            /*
             * if (SageValue.ToLong(counter) == long.MaxValue())
             * {
             *    break;
             * }
             */
            statements.Add(IfStatement(
                BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    TranspilerExtensions.GetLongValue(_transpiler.Runtime.GetFromRuntime(assignmentVariableName)),
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        PredefinedType(
                            Token(SyntaxKind.LongKeyword)),
                        IdentifierName("MaxValue"))),
                TranspilerExtensions.BlockWithHiddenDirectives(
                    SingletonList<StatementSyntax>(
                        BreakStatement())))
                .WithHiddenLineDirective());
        }
        else if (context.Downto() != null)
        {
            /*
             * if (counter.ToLong() == long.MinValue())
             * {
             *    break;
             * }
             */
            statements.Add(IfStatement(
                BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    TranspilerExtensions.GetLongValue(_transpiler.Runtime.GetFromRuntime(assignmentVariableName)),
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        PredefinedType(
                            Token(SyntaxKind.LongKeyword)),
                        IdentifierName("MinValue"))),
                TranspilerExtensions.BlockWithHiddenDirectives(
                    SingletonList<StatementSyntax>(
                        BreakStatement())))
                .WithHiddenLineDirective());
        }

        /*
         * WHILE({assignment} {kind} {target})
         * {
         *     {statement}
         * }
         */
        results.Add(WhileStatement(
            BinaryExpression(
                kindCheck,
                TranspilerExtensions.GetLongValue(_transpiler.Runtime.GetFromRuntime(assignmentVariableName)),
                TranspilerExtensions.GetLongValue(_transpiler.Runtime.GetFromRuntime(targetVariableName))),
            TranspilerExtensions.BlockWithHiddenDirectives(statements))
            .WithLineDirective(context.For().Symbol, context.Do().Symbol, _transpiler.SourceFileName));

        return results;
    }

    /// <summary>
    /// Visits the statement for setting variables: SET @var='bar'
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitSetVariable(SageParser.SetVariableContext context)
    {
        string variableName = context.variableAssignment().VarName().GetText();

        ExpressionSyntax expression = _transpiler.ExpressionVisitor.Visit(context.variableAssignment().expression());

        yield return _transpiler.Runtime.AssignToRuntime(variableName, expression).WithLineDirective(context, this._transpiler.SourceFileName);
    }

    /// <summary>
    /// Visits the variable declaration: VAR @foo, @bar
    /// Documentation specifies that this resets the variable to NULL if encountered.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitVarDeclaration(SageParser.VarDeclarationContext context)
    {
        for (int i = 0; i < context.VarName().Length; i++)
        {
            yield return _transpiler.Runtime.AssignToRuntime(
                    context.VarName(i).GetText(),
                    LiteralExpression(SyntaxKind.NullLiteralExpression))
                .WithLineDirective(context, this._transpiler.SourceFileName);
        }
    }

    /// <summary>
    /// Function calls can show up as either expressions (right side of = sign), or statements (all by themselves on a line).
    ///
    /// This visit is when it was a statement, it just forwards it to the expression visitor.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitFunctionCall(SageParser.FunctionCallContext context)
    {
        return new[]
        {
            ExpressionStatement(_transpiler.ExpressionVisitor.VisitFunctionCall(context))
                .WithLineDirective(context.Start, context.Stop, _transpiler.SourceFileName)
        };
    }

    /// <summary>
    /// A constant expression in the statement visitor means something like any of the following lines:
    /// %%[
    /// @var
    /// 3
    /// 7
    /// ]%%
    /// A constant expression statement doesn't make sense in the grammar, but the existing Marketing Cloud AMPscript interpreter ignores these lines.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitConstantExpression(SageParser.ConstantExpressionContext context)
    {
        // TODO: Add to warning stream that this will not do what the user expects.
        return Enumerable.Empty<StatementSyntax>();
    }

    /// <summary>
    /// A comparision expression in the statement visitor means something like any of the following lines:
    /// %%[
    /// 5 < 5
    /// @foo < "bar"
    /// ]%%
    /// A compression expression statement doesn't make sense in the grammar, but the existing Marketing Cloud AMPscript interpreter ignores these lines.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitComparisionExpression(SageParser.ComparisionExpressionContext context)
    {
        // TODO: Add to warning stream that this will not do what the user expects.
        return Enumerable.Empty<StatementSyntax>();
    }

    /// <summary>
    /// A logical expression in the statement visitor means something like any of the following lines:
    /// %%[
    /// 4 || 5
    /// 6 && 10
    /// ]%%
    /// A logical expression statement doesn't make sense in the grammar, but the existing Marketing Cloud AMPscript interpreter ignores these lines.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitLogicalExpression(SageParser.LogicalExpressionContext context)
    {
        // TODO: Add to warning stream that this will not do what the user expects.
        return Enumerable.Empty<StatementSyntax>();
    }

    /// <summary>
    /// A parenthesis expression in the statement visitor means something like any of the following lines:
    /// %%[
    /// (4 || 5)
    /// (6 < 10)
    /// ]%%
    /// A parenthesis expression statement doesn't make sense in the grammar, but the existing Marketing Cloud AMPscript interpreter ignores these lines.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitParenthesisExpression(SageParser.ParenthesisExpressionContext context)
    {
        // TODO: Add to warning stream that this will not do what the user expects.
        return Enumerable.Empty<StatementSyntax>();
    }

    /// <summary>
    /// An attribute expression in the statement visitor means something like any of the following lines:
    /// %%[
    /// [First name]
    /// ]%%
    /// An attribute expression statement doesn't make sense in the grammar, but the existing Marketing Cloud AMPscript interpreter ignores these lines.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitAttributeExpression(SageParser.AttributeExpressionContext context)
    {
        // TODO: Add to warning stream that this will not do what the user expects.
        return Enumerable.Empty<StatementSyntax>();
    }
}
