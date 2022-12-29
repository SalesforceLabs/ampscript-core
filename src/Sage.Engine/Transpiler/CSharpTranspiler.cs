// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Sage.Engine.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Sage.Engine.Transpiler;

/// <summary>
/// This is the main entrypoint into the transpilation process.
///
/// It holds all visitors responsible for visiting the parse tree created by ANTLR, as well as
/// the variable helper to get & set variables at scope.
/// </summary>
internal class CSharpTranspiler
{
    internal Runtime Runtime
    {
        get;
    }

    internal BlockVisitor BlockVisitor
    {
        get;
    }

    internal StatementVisitor StatementVisitor
    {
        get;
    }

    internal ExpressionVisitor ExpressionVisitor
    {
        get;
    }

    internal StringVisitor StringVisitor
    {
        get;
    }

    private readonly SageParser _parser;

    internal CSharpTranspiler(SageParser parser)
    {
        BlockVisitor = new BlockVisitor(this);
        StatementVisitor = new StatementVisitor(this);
        ExpressionVisitor = new ExpressionVisitor(this);
        StringVisitor = new StringVisitor(this);
        Runtime = new Runtime();
        this._parser = parser;
    }

    /// <summary>
    /// Provides a C# method for a block of code passed to this transpiler
    /// </summary>
    internal LocalFunctionStatementSyntax GenerateMethodFromCode(string methodName)
    {
        var statements = new List<StatementSyntax>();
        statements.Add(Runtime.GetInitializationStatement());
        statements.AddRange(this.BlockVisitor.Visit(this._parser.contentBlock()));

        // public static string Method(RuntimeContext __runtime)
        return LocalFunctionStatement(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier(methodName))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                            Parameter(
                                    Identifier(Runtime.RuntimeVariable))
                                .WithType(
                                    IdentifierName("RuntimeContext")))))
            .WithBody(
                Block(statements));
    }
}