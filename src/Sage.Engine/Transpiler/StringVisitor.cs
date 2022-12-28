// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;

namespace Sage.Engine.Transpiler;

/// <summary>
/// A specialized visitor to generate a string literal expression
/// Strings in AMPscript can either be single or double quoted.
/// A double quoted string can escape the double quote character with two double quotes.
/// A single quoted string can escape the single quote character with two single quotes.
/// For example, imagine the code:
///     SET @whoseFood = 'Logan''s food'
///
/// This would produce the string: Logan's food
/// </summary>
internal class StringVisitor : SageParserBaseVisitor<LiteralExpressionSyntax>
{
    private readonly CSharpTranspiler _transpiler;

    internal StringVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    /// <summary>
    /// Visits single quoted strings, such as 'Favorite Code'.
    /// Single quoted strings can be escaped with two single quotes, such as 'Paul''s favorite code'
    /// </summary>
    public override LiteralExpressionSyntax VisitSingleQuoteString(SageParser.SingleQuoteStringContext context)
    {
        var joinedStrings = new StringBuilder();

        foreach (IParseTree child in context.children)
        {
            if (child is not ITerminalNode terminalNode)
            {
                continue;
            }

            if (terminalNode.Symbol.Type == SageLexer.SingleQuoteStringData)
            {
                joinedStrings.Append(terminalNode.GetText());
            }
            else if (terminalNode.Symbol.Type == SageLexer.EscapedSingleQuote)
            {
                joinedStrings.Append("'");
            }
        }
        string result = joinedStrings.ToString();

        return SyntaxFactory.LiteralExpression(
            SyntaxKind.StringLiteralExpression,
            SyntaxFactory.Literal(result));
    }

    /// <summary>
    /// Visits double  quoted strings, such as "The best food".
    /// Double quoted strings can be escaped with two double quotes, such as "The ""Best"" food?"
    /// </summary>
    public override LiteralExpressionSyntax VisitDoubleQuoteString(SageParser.DoubleQuoteStringContext context)
    {
        var joinedStrings = new StringBuilder();

        foreach (IParseTree child in context.children)
        {
            if (child is not ITerminalNode terminalNode)
            {
                continue;
            }

            if (terminalNode.Symbol.Type == SageLexer.DoubleQuoteStringData)
            {
                joinedStrings.Append(terminalNode.GetText());
            }
            else if (terminalNode.Symbol.Type == SageLexer.EscapedDoubleQuote)
            {
                joinedStrings.Append("\"");
            }
        }

        string result = joinedStrings.ToString();

        return SyntaxFactory.LiteralExpression(
            SyntaxKind.StringLiteralExpression,
            SyntaxFactory.Literal(result));
    }
}