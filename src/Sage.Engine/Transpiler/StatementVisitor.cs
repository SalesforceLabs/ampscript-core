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
    /// Visits the statement for setting variables: SET @var='bar'
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitSetVariable(SageParser.SetVariableContext context)
    {
        string variableName = context.variableAssignment().VarName().GetText();

        ExpressionSyntax expression = _transpiler.ExpressionVisitor.Visit(context.variableAssignment().expression());

        yield return _transpiler.Runtime.AssignToRuntime(variableName, expression);
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
}
