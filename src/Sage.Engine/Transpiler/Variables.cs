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
/// Holds the runtime representation of the variable symbol table.
///
/// Normally, a symbol table would be a stack where they are pushed & popped
/// based on the scope - but in AMPscript, the symbols leak everywhere and once set in one part of code, they become
/// available everywhere. So, the representation is just Dictionary<string, object>
/// </summary>
internal class Variables
{
    internal const string VariableSymbolName = "__variables";

    /// <summary>
    /// Returns the C# equivalent to returning the variable declaration & initialization, such as:
    /// Dictionary<string, object> __variables = new Dictionary<string, object>
    /// </summary>
    /// <param name="variableName">Name of the variable</param>
    /// <returns>A statement declaring the variable dictionary</returns>
    internal static StatementSyntax GetInitializationStatement()
    {
        return LocalDeclarationStatement(
            VariableDeclaration(
                GenericName(
                        Identifier("Dictionary"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    PredefinedType(
                                        Token(SyntaxKind.StringKeyword)),
                                    Token(SyntaxKind.CommaToken), PredefinedType(
                                        Token(SyntaxKind.ObjectKeyword))
                                }))))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                                Identifier(VariableSymbolName))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                            GenericName(
                                                    Identifier("Dictionary"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SeparatedList<TypeSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                PredefinedType(
                                                                    Token(SyntaxKind.StringKeyword)),
                                                                Token(SyntaxKind.CommaToken),
                                                                PredefinedType(
                                                                    Token(SyntaxKind.ObjectKeyword))}))))
                                        .WithArgumentList(
                                            ArgumentList()))))));
    }

    /// <summary>
    /// Assigns an expression to a variable. The variableName must be AMPscript name, such as "@FOO".
    /// </summary>
    internal AssignmentExpressionSyntax AssignToRuntime(string variableName, ExpressionSyntax right)
    {
        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            ElementAccessExpression(
                    IdentifierName(VariableSymbolName))
                .WithArgumentList(
                    BracketedArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal(ConvertAmpVariableNameToCSharpVariableName(variableName)))
                            )))),
            right);

    }

    /// <summary>
    /// Returns the value of a variable from the runtime. The variableName must be AMPscript name, such as "@FOO".
    /// </summary>
    internal ElementAccessExpressionSyntax GetFromRuntime(string variableName)
    {
        return ElementAccessExpression(
                IdentifierName(VariableSymbolName))
            .WithArgumentList(
                BracketedArgumentList(
                    SingletonSeparatedList<ArgumentSyntax>(
                        Argument(LiteralExpression(
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
}
