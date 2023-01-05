// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;
using Antlr4.Runtime.Tree;

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
        return node.WithLineDirective(context.Start, context.Stop, filename);
    }

    /// <summary>
    /// Decorates the provided syntax node with a #line directive, indicating which line in the source file this represents.
    /// </summary>
    /// <see cref="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/enhanced-line-directives.md"/>
    public static TSyntax WithLineDirective<TSyntax>(
        this TSyntax node,
        ITerminalNode context, string filename) where TSyntax : SyntaxNode
    {
        return node.WithLineDirective(context.Symbol, context.Symbol, filename);
    }

    /// <summary>
    /// Decorates the provided syntax node with a #line directive, indicating which line in the source file this represents.
    /// </summary>
    /// <see cref="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/enhanced-line-directives.md"/>
    public static TSyntax WithLineDirective<TSyntax>(
        this TSyntax node,
        IToken start, IToken stop, string filename) where TSyntax : SyntaxNode
    {
        if (string.IsNullOrEmpty(filename))
        {
            return node;
        }

        return node.WithLeadingTrivia(TriviaList(Trivia(GetSpanFromTokens(start, stop, filename))));
    }

    /// <summary>
    /// Provides the #line directive from the start and stop tokens
    /// </summary>
    /// <see cref="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/enhanced-line-directives.md"/>
    private static LineSpanDirectiveTriviaSyntax GetSpanFromTokens(IToken start, IToken stop, string filename)
    {
        // +1 because these are zero-based and the trivia expects 1-based data
        return LineSpanDirectiveTrivia(
            LineDirectivePosition(
                Literal(start.Line),
                Literal(start.Column + 1)),
            LineDirectivePosition(
                Literal(stop.Line),
                // The stop column is the first character in the column - to get to the end it needs to add the length of the token
                Literal(stop.Column + stop.Text.Length + 1)),
            Literal(filename),
            true);
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

    /// <summary>
    /// Invokes a static method on a given class. Arguments can be added to the returned InvocationExpression
    /// </summary>
    /// <param name="className">Name of the class, without the namespace</param>
    /// <param name="method">The method in the class to invoke</param>
    /// <returns>className.method()</returns>
    public static InvocationExpressionSyntax InvokeStaticMethodOnClass(string className, string method)
    {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(className),
                IdentifierName(method)));
    }

    /// <summary>
    /// Converts the result of the provided expression to a long using SageValue.ToLong
    /// </summary>
    /// <param name="inputExpression">The input expression to be converted to a C# long</param>
    /// <returns>SageValue.ToLong(expression)</returns>
    public static ExpressionSyntax GetLongValue(ExpressionSyntax inputExpression)
    {
        return InvokeStaticMethodOnClass("SageValue", "ToLong")
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(inputExpression))));
    }
}

