// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Sage.Engine.Transpiler;

/// <summary>
/// Visits expressions, which are values and functions that generate new values
/// These are anything in the parser that roll up to the expression node.
/// </summary>
internal class ExpressionVisitor : SageParserBaseVisitor<ExpressionSyntax>
{
    private readonly CSharpTranspiler _transpiler;

    public ExpressionVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    /// <summary>
    /// Visits expressions within parenthesis ()
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override ExpressionSyntax VisitParenthesisExpression(SageParser.ParenthesisExpressionContext context)
    {
        return base.Visit(context.parentheses().expression());
    }

    /// <summary>
    /// A constant expression is either the variable name, or a constant.
    /// </summary>
    public override ExpressionSyntax VisitConstantExpression(SageParser.ConstantExpressionContext context)
    {
        if (context.VarName() != null)
        {
            return _transpiler.Runtime.GetFromRuntime(context.VarName().GetText());
        }
        else
        {
            return base.Visit(context.constant());
        }
    }

    /// <summary>
    /// This visits anything that's recognized as a constant, such as an int, string, etc.
    /// </summary>
    public override ExpressionSyntax VisitConstant(SageParser.ConstantContext context)
    {
        if (context.Integer() != null)
        {
            if (!long.TryParse(context.GetText(), out long longResult))
            {
                longResult = 0;
                // TODO ERRORSTREAM ADD ERROR throw new ParserException(context, "Integer not parsable by a long");
            }

            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(longResult));
        }

        if (context.Real() != null)
        {
            if (!decimal.TryParse(context.GetText(), out decimal decimalResult))
            {
                throw new InternalEngineException($"Unable to parse {context.GetText()} as decimal");
            }

            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(decimalResult));
        }

        if (context.True() != null)
        {
            return LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }

        if (context.False() != null)
        {
            return LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }

        if (context.@string() != null)
        {
            return _transpiler.StringVisitor.Visit(context.@string());
        }

        throw new InternalEngineException("Unknown constant during parsing");
    }

    /// <summary>
    /// Visits an AMPscript function call
    ///
    /// Functions are supported by methods in the Sage.Engine.Functions namespace, with the name in all uppercase:
    /// OUTPUT(), V(), NOW(), etc.
    ///
    /// All supported functions must take the variables and output stream as the first two parameters:
    /// FUNCTION(Dictionary<string, object> __variables, StringBuilder __outputStream, ...)
    ///
    /// Functions that have data to return must return of type object.
    /// </summary>
    public override ExpressionSyntax VisitFunctionCall(SageParser.FunctionCallContext context)
    {
        string functionName = context.NameString().GetText().ToUpper();

        var argSyntax = new List<ArgumentSyntax>();

        for (int i = 0; i < context.arguments().expression().Length; i++)
        {
            argSyntax.Add(Argument(base.Visit(context.arguments().expression(i))));
        }

        // This does not add any #line directives, since it's just an expression.
        // The invocation of the function as an ExpressionStatement adds the #line directive, since usually you want to break
        // and debug on a statement and not an expression.
        //
        // For example, debugging a function call like this:
        // OUTPUT(CONCAT("A","B"))
        // Do you want to F10 into each and every argument? Probably not - so don't put line directives on the expressions or arguments.
        return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(Runtime.RuntimeVariable),
                        IdentifierName(functionName)))
            .WithArgumentList(ArgumentList(SeparatedList(argSyntax)));
    }

    /// <summary>
    /// Visits a comparision, such as <= >=, ==, etc.
    /// </summary>
    public override ExpressionSyntax VisitComparisionExpression(SageParser.ComparisionExpressionContext context)
    {
        ExpressionSyntax leftExpression = base.Visit(context.expression(0));
        ExpressionSyntax rightExpression = base.Visit(context.expression(1));

        string methodName;
        switch (context.op.Type)
        {
            case SageLexer.LessThan:
                methodName = "LessThan";
                break;
            case SageLexer.LessThanOrEqualTo:
                methodName = "LessThanOrEqual";
                break;
            case SageLexer.EqualTo:
                methodName = "EqualTo";
                break;
            case SageLexer.NotEqual:
                methodName = "NotEqualTo";
                break;
            case SageLexer.GreaterThanOrEqualTo:
                methodName = "GreaterThanOrEqual";
                break;
            case SageLexer.GreaterThan:
                methodName = "GreaterThan";
                break;
            default:
                throw new InternalEngineException(
                    $"Unable to parse expression op type {SageLexer.DefaultVocabulary.GetDisplayName(context.op.Type)}");
        }

        // SageValue.{Comparison}(left, right);
        // Example: SageValue.EqualTo(left, right);
        return TranspilerExtensions.InvokeStaticMethodOnClass("SageValue", methodName)
            .WithArgumentList(
                ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            Argument(leftExpression),
                            Token(SyntaxKind.CommaToken),
                            Argument(rightExpression)
                        })));
    }

    /// <summary>
    /// Visits && or || boolean expression
    /// </summary>
    public override ExpressionSyntax VisitLogicalExpression(SageParser.LogicalExpressionContext context)
    {
        // SageValue.ToBoolean(left)
        ExpressionSyntax leftExpression = TranspilerExtensions.GetBoolValue(base.Visit(context.expression(0)));

        // SageValue.ToBoolean(right)
        ExpressionSyntax rightExpression = TranspilerExtensions.GetBoolValue(base.Visit(context.expression(1)));

        SyntaxKind kind;
        switch (context.op.Type)
        {
            case SageParser.BooleanAnd:
                kind = SyntaxKind.LogicalAndExpression;
                break;
            case SageParser.BooleanOr:
                kind = SyntaxKind.LogicalOrExpression;
                break;
            default:
                throw new InternalEngineException(
                    $"Unable to parse expression op type {SageParser.DefaultVocabulary.GetDisplayName(context.op.Type)}");
        }

        // Ex: SageValue.ToBoolean(left) || SageValue.ToBoolean(right)
        return BinaryExpression(
            kind,
            leftExpression,
            rightExpression);
    }

    public override ExpressionSyntax VisitNegateExpression(SageParser.NegateExpressionContext context)
    {
        ExpressionSyntax expression = TranspilerExtensions.GetBoolValue(base.Visit(context.expression()));

        return PrefixUnaryExpression(
            SyntaxKind.LogicalNotExpression,
            ParenthesizedExpression(expression));
    }

    public override ExpressionSyntax VisitAttribute(SageParser.AttributeContext context)
    {
        string attributeName = context.GetText().TrimStart('[').TrimEnd(']');

        return TranspilerExtensions.GetAttributeValue(attributeName);
    }
}
