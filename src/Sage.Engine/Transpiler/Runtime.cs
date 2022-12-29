// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Sage.Engine.Transpiler;

/// <summary>
/// Manages operations against the runtime context, such as variables, output stream, HTTP request, etc.
/// </summary>
internal class Runtime
{
    internal const string RuntimeVariable = "__runtime";

    /// <summary>
    /// Returns the C# equivalent to returning the variable declaration & initialization, such as:
    /// RuntimeContext __runtime = new RuntimeContext();
    /// </summary>
    /// <param name="variableName">Name of the variable</param>
    /// <returns>A statement declaring the variable dictionary</returns>
    internal static StatementSyntax GetInitializationStatement()
    {
        return LocalDeclarationStatement(
            VariableDeclaration(
                    IdentifierName("RuntimeContext"))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                                Identifier(RuntimeVariable))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                            IdentifierName("RuntimeContext"))
                                        .WithArgumentList(
                                            ArgumentList()))))));
    }

    /// <summary>
    /// Assigns an expression to a variable. The variableName must be AMPscript name, such as "@FOO".
    /// </summary>
    internal StatementSyntax AssignToRuntime(string variableName, ExpressionSyntax right)
    {
        return ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(RuntimeVariable),
                        IdentifierName("SetVariable")))
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(ConvertAmpVariableNameToCSharpVariableName(variableName)))),
                                Token(SyntaxKind.CommaToken),
                                Argument(right)
                            }))));

    }

    /// <summary>
    /// Returns the value of a variable from the runtime. The variableName must be AMPscript name, such as "@FOO".
    /// </summary>
    internal ExpressionSyntax GetFromRuntime(string variableName)
    {
        return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(RuntimeVariable),
                        IdentifierName("GetVariable")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal(ConvertAmpVariableNameToCSharpVariableName(variableName)))))));
    }

    /// <summary>
    /// AMPscript variables are case-insensitive, and C# is not.
    /// This function takes an AMPscript variable aka @FoO, and makes it the C# string, aka @foo
    /// </summary>
    /// <param name="ampVariableName">AMPscript representation of the variable, with the @ sign at the front</param>
    /// <returns>The C# name of that variable</returns>
    /// <exception cref="InternalEngineException">If the variable does not start with @</exception>
    internal static string ConvertAmpVariableNameToCSharpVariableName(string ampVariableName)
    {
        return ampVariableName.ToLower();
    }

    /// <summary>
    /// Emits a string to the output stream.
    ///
    /// The output stream is used to output text to the final result of executing the file.
    /// </summary>
    internal ExpressionStatementSyntax EmitToOutputStream(string content)
    {
        // __runtime.Output(content);
        return ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(RuntimeVariable),
                        IdentifierName("Output")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal(content)))))));
    }
}
