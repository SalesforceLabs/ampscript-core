// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis.CSharp;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Sage.Engine.Transpiler;

/// <summary>
/// Visits AMPscript IF statements and returns the corresponding C# representation of it
/// </summary>
internal class IfVisitor : SageParserBaseVisitor<IfStatementSyntax>
{
    private readonly CSharpTranspiler _transpiler;

    public IfVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    private IfStatementSyntax GetIf(ExpressionSyntax expression, IEnumerable<StatementSyntax> block)
    {
        return IfStatement(
            expression,
            TranspilerExtensions.BlockWithHiddenDirectives(block));
    }

    public override IfStatementSyntax VisitIfStatement(SageParser.IfStatementContext context)
    {
        ExpressionSyntax e = _transpiler.ExpressionVisitor.Visit(context.expression());
        return GetIf(e, _transpiler.BlockVisitor.Visit(context.contentBlock())).WithLineDirective(context.expression(), this._transpiler.SourceFileName);
    }

    public override IfStatementSyntax VisitElseIfStatement(SageParser.ElseIfStatementContext context)
    {
        return GetIf(_transpiler.ExpressionVisitor.Visit(context.expression()), _transpiler.BlockVisitor.Visit(context.contentBlock())).WithLineDirective(context.expression(), this._transpiler.SourceFileName);
    }
}
