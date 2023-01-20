// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Sage.Engine.Parser;

namespace Sage.Engine.Transpiler;

/// <summary>
/// This visits blocks of AMPScript code, such as the contents of a file, what's inside an IF statement, or what's in a DO loop.
/// </summary>
internal class BlockVisitor : SageParserBaseVisitor<IEnumerable<StatementSyntax>>
{
    private readonly CSharpTranspiler _transpiler;

    public BlockVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    /// <summary>
    /// A content block can be HTML, AMP, SSJS, GUIDE - anything!
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitContentBlock(SageParser.ContentBlockContext context)
    {
        foreach (SageParser.AmpOrEmbeddedContentContext block in context.ampOrEmbeddedContent())
        {
            foreach (StatementSyntax statement in base.Visit(block))
            {
                yield return statement;
            }
        }
    }

    /// <summary>
    /// An AmpBlock is an ampscript block contained within the %%[ ]%% braces.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitAmpBlock(SageParser.AmpBlockContext context)
    {
        for (int i = 0; i < context.ampStatement().Length; i++)
        {
            foreach (StatementSyntax statement in _transpiler.StatementVisitor.Visit(context.ampStatement(i)))
            {
                yield return statement;
            }
        }
    }

    public override IEnumerable<StatementSyntax> VisitInlineAmpBlock(SageParser.InlineAmpBlockContext context)
    {
        return new[]
        {
            _transpiler.Runtime.EmitToOutputStream(_transpiler.ExpressionVisitor.Visit(context.expression()))
                .WithLineDirective(context, _transpiler.SourceFileName)
        };
    }

    /// <summary>
    /// Inline HTML only needs to add the content to the output stream.
    ///
    /// Example:
    /// __outputStream.Append("<html><body></body></html>");
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitInlineHtml(SageParser.InlineHtmlContext context)
    {
        return new[]
        {
            _transpiler.Runtime.EmitToOutputStream(context.GetText())
        };
    }

    public override IEnumerable<StatementSyntax> VisitAttributeNameAtSea(SageParser.AttributeNameAtSeaContext context)
    {
        string attributeName = context.GetText().TrimStart('%').TrimEnd('%');

        // This does not add any #line directives, since it's just an expression.
        // The invocation of the function as an ExpressionStatement adds the #line directive, since usually you want to break
        // and debug on a statement and not an expression.
        //
        // For example, debugging a function call like this:
        // OUTPUT(CONCAT("A","B"))
        // Do you want to F10 into each and every argument? Probably not - so don't put line directives on the expressions or arguments.
        return new[]
        {
            _transpiler.Runtime.EmitToOutputStream(TranspilerExtensions.GetAttributeValue(attributeName))
        };
    }
}
