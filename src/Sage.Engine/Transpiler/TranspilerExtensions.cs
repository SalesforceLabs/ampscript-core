// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace Sage.Engine.Transpiler;

internal static class TranspilerExtensions
{
    /// <summary>
    /// Decorates the provided syntax node with a #line directive, indicating which line in the source file this represents.
    /// </summary>
    /// <see cref="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/enhanced-line-directives.md"/>
    public static TSyntax WithLineDirective<TSyntax>(
        this TSyntax node,
        ParserRuleContext context, string? filename) where TSyntax : SyntaxNode
    {
        if (string.IsNullOrEmpty(filename))
        {
            return node;
        }

        // +1 because these are zero-based and the trivia expects 1-based data
        LineSpanDirectiveTriviaSyntax trivia = LineSpanDirectiveTrivia(
            LineDirectivePosition(
                Literal(context.Start.Line),
                Literal(context.Start.Column + 1)),
            LineDirectivePosition(
                Literal(context.Stop.Line),
                // The stop column is the first character in the column - to get to the end it needs to add the length of the token
                Literal(context.Stop.Column + context.Stop.Text.Length + 1)),
            Literal(filename),
            true);

        return node.WithLeadingTrivia(TriviaList(Trivia(trivia)));
    }

    /// <summary>
    /// Decorates the provided syntax node with a line directive telling the compiler that it should skip this code.
    /// aka #line hidden
    /// </summary>
    public static TSyntax WithHiddenLineDirective<TSyntax>(
        this TSyntax node) where TSyntax : SyntaxNode
    {
        return node.WithLeadingTrivia(TriviaList(Trivia(LineDirectiveTrivia(Token(SyntaxKind.HiddenKeyword), true))));
    }
}

