// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;

namespace Sage.Engine.Transpiler;

/// <summary>
/// A special visitor to handle inline HTML and the scripting tags.
/// </summary>
internal class HtmlVisitor : SageParserBaseVisitor<IEnumerable<StatementSyntax>>
{
    private readonly CSharpTranspiler _transpiler;

    public HtmlVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    protected override IEnumerable<StatementSyntax> AggregateResult(IEnumerable<StatementSyntax> aggregate, IEnumerable<StatementSyntax> nextResult)
    {
        var next = nextResult ?? Enumerable.Empty<StatementSyntax>();
        return aggregate?.Concat(next) ?? next;
    }

    /// <summary>
    /// A script tag may or may not want to be emitted.
    ///
    /// If it was an AMPscript script tag, then it shouldn't be emitted.  All other script tags should be emitted.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override IEnumerable<StatementSyntax> VisitScriptTag(SageParser.ScriptTagContext context)
    {
        foreach (var child in context.children)
        {
            if (context.GetText().Contains("ampscript", StringComparison.OrdinalIgnoreCase))
            {
                return Enumerable.Empty<StatementSyntax>();
            }
        }
        StringBuilder builder = new StringBuilder();

        foreach (var child in context.children)
        {
            builder.Append(child.GetText());
        }

        return new[]
        {
            _transpiler.Runtime.EmitToOutputStream(builder.ToString())
        };
    }

    /// <summary>
    /// By default, all terminal nodes from inline HTML should be emitted to the output stream.
    ///
    /// The only special case is the scripting tags, which is handled above.
    /// </summary>
    public override IEnumerable<StatementSyntax> VisitTerminal(ITerminalNode node)
    {
        return new[]
        {
            _transpiler.Runtime.EmitToOutputStream(node.GetText())
        };
    }
}
