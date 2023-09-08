// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Antlr4.Runtime;

namespace Sage.Engine.Transpiler;

/// <summary>
/// This is the main entrypoint into the transpilation process.
///
/// It holds all visitors responsible for visiting the parse tree created by ANTLR, as well as
/// the variable helper to get & set variables at scope.
/// </summary>
internal class CSharpTranspiler
{
    /// <summary>
    /// This is the full path to the input file
    /// </summary>
    internal string SourceFileName
    {
        get;
    }

    /// <summary>
    /// This is the name of the main method to be generated
    /// </summary>
    internal string GeneratedMethod
    {
        get;
    }
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

    internal IfVisitor IfVisitor
    {
        get;
    }


    internal GuideVisitor GuideVisitor
    {
        get;
    }

    public HtmlVisitor HtmlVisitor
    {
        get;
    }

    private readonly SageParser _parser;

    /// <summary>
    /// Generates a transpiler without a backing source file.
    /// </summary>
    /// <remarks>No #line directives are given</remarks>
    internal CSharpTranspiler(SageParser parser)
    {
        BlockVisitor = new BlockVisitor(this);
        StatementVisitor = new StatementVisitor(this);
        ExpressionVisitor = new ExpressionVisitor(this);
        StringVisitor = new StringVisitor(this);
        GuideVisitor = new GuideVisitor(this);
        HtmlVisitor = new HtmlVisitor(this);
        Runtime = new Runtime();
        IfVisitor = new IfVisitor(this);
        this.SourceFileName = "TEST.cs";
        this.GeneratedMethod = "TEST";
        this._parser = parser;
    }

    /// <summary>
    /// Creates a transpiler based on the passed in parser.
    /// The filename is used to generate #line directives.
    /// </summary>
    internal CSharpTranspiler(string fileName, string generatedMethodName, SageParser parser) : this(parser)
    {
        SourceFileName = fileName;
        this.GeneratedMethod = generatedMethodName;
    }

    /// <summary>
    /// Creates a transpiler based on the input source code
    /// </summary>
    internal static CSharpTranspiler CreateFromSource(string code)
    {
        return CreateFromSource("TEST", "TEST", code);
    }

    /// <summary>
    /// Creates a transpiler based on the input source code
    /// </summary>
    internal static CSharpTranspiler CreateFromSource(string sourceFile, string generatedMethodName, string code)
    {
        var stream = new AntlrInputStream(code);
        var lexer = new SageLexer(stream);
        var tokens = new CommonTokenStream(lexer);

        return new CSharpTranspiler(sourceFile, generatedMethodName, new SageParser(tokens));
    }

    /// <summary>
    /// Generates the namespace, usings and static class with all methods for the executable
    /// </summary>
    internal CompilationUnitSyntax GenerateCode()
    {
        var methods = new List<MemberDeclarationSyntax>
        {
            GenerateMethodFromCode(GeneratedMethod)
        };

        /*
         * namespace Sage.Engine.Runtime
         * {
         *     public static class AmpProgram
         *     {
         *         ** Insert methods here **
         *     }
         * }
         */
        return CompilationUnit()
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    NamespaceDeclaration(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("Sage"),
                                    IdentifierName("Engine")),
                                IdentifierName("Runtime")))
                        .WithUsings(List(
                            new[]
                            {
                                UsingDirective(
                                    QualifiedName(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            IdentifierName("Collections")),
                                        IdentifierName("Generic"))),
                                UsingDirective(
                                    QualifiedName(
                                        IdentifierName("System"),
                                        IdentifierName("Text"))),
                                UsingDirective(
                                    IdentifierName("System"))
                            }))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration("AmpProgram")
                                    .WithModifiers(
                                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                                    .WithMembers(List(methods))))));
    }

    /// <summary>
    /// Provides a C# method for a block of code passed to this transpiler
    /// </summary>
    internal MethodDeclarationSyntax GenerateMethodFromCode(string methodName)
    {
        var statements = new List<StatementSyntax>();
        statements.AddRange(this.BlockVisitor.Visit(this._parser.contentBlock()));

        // public static string Method(RuntimeContext __runtime)
        return MethodDeclaration(
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
                Block(statements))
            .WithHiddenLineDirective();
    }
}