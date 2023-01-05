// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;

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

            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(longResult));
        }

        if (context.Real() != null)
        {
            if (!decimal.TryParse(context.GetText(), out decimal decimalResult))
            {
                throw new InternalEngineException($"Unable to parse {context.GetText()} as decimal");
            }

            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(decimalResult));
        }

        if (context.True() != null)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }

        if (context.False() != null)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }

        if (context.@string() != null)
        {
            return _transpiler.StringVisitor.Visit(context.@string());
        }

        throw new InternalEngineException("Unknown constant during parsing");
    }
}
