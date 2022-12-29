// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using System.Reflection.Emit;
    using Antlr4.Runtime;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Sage.Engine.Parser;
    using Sage.Engine.Transpiler;

    /// <summary>
    /// These tests validate that the ANTLR4 generated parse tree is what is cSharpExpected from test files
    /// </summary>
    [TestFixture]
    public class TranspilerTests
    {
        private static SageParser GetParserInAmpMode(string code)
        {
            return GetParserInMode(code, SageLexer.AMP);
        }

        private static SageParser GetParserInContentMode(string code)
        {
            return GetParserInMode(code, 0);
        }

        private static SageParser GetParserInMode(string code, int mode)
        {
            ICharStream stream = new AntlrInputStream(code);
            SageLexer lexer = new SageLexer(stream);
            lexer.Mode(mode);
            ITokenStream tokens = new CommonTokenStream(lexer);
            return new SageParser(tokens);
        }

        [Test]
        public void TestVariableDeclaration()
        {
            string variableDeclaration = Runtime.GetInitializationStatement().NormalizeWhitespace().ToString();
            string expected =
                $"RuntimeContext {Runtime.RuntimeVariable} = new RuntimeContext();";

            Assert.That(variableDeclaration, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("\"foo\"", "foo")]
        [TestCase("'foo'", "foo")]
        [TestCase("'singlequote''s'", "singlequote's")]
        [TestCase("'how about \"no\"'", "how about \"no\"")]
        [TestCase("\"how about \"\"no\"\"\"", "how about \"no\"")]
        public void TestStringLiterals(string code, string expected)
        {
            SageParser parser = GetParserInAmpMode(code);
            var transpiler = new CSharpTranspiler(parser);

            LiteralExpressionSyntax transpiledString = transpiler.StringVisitor.Visit(parser.@string());
            Assert.That(transpiledString.Token.Value?.ToString(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("SET @FOO = 1", $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", 1L);")]
        [TestCase("SET @BAR = 'Foo'", $"{Runtime.RuntimeVariable}.SetVariable(\"@bar\", \"Foo\");")]
        [TestCase("SET @BAR = @FOO", $"{Runtime.RuntimeVariable}.SetVariable(\"@bar\", {Runtime.RuntimeVariable}.GetVariable(\"@foo\"));")]
        public void TestSet(string code, string expected)
        {
            SageParser parser = GetParserInAmpMode(code);
            var transpiler = new CSharpTranspiler(parser);

            TestAmpCodeGeneratesExpectedCSharpCode(new[] { expected }, parser.setVariable(), transpiler.StatementVisitor);
        }

        [Test]
        [TestCase("1", "1L")]
        [TestCase("'Foo'", "\"Foo\"")]
        [TestCase("10.0", "10.0M")]
        [TestCase("true", "true")]
        [TestCase("false", "false")]
        [TestCase("'true'", "\"true\"")]
        [TestCase("'false'", "\"false\"")]
        [TestCase("922337203685477580711", "0L")] // Overflow a long, you get NOTHING!
        public void TestSetConstants(string ampInput, string cSharpExpected)
        {
            string ampCodeInput = $"SET @foo = {ampInput}";
            string cSharpCodeExpected = $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", {cSharpExpected});";
            SageParser parser = GetParserInAmpMode(ampCodeInput);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(new[] { cSharpCodeExpected }, parser.setVariable(), transpiler.StatementVisitor);
        }

        [Test]
        [TestCase("VAR @FOO", new[]
        {
            $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", null);"
        })]
        [TestCase("VAR @FOO, @BAR, @BIZ", new[]
        {
            $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", null);",
            $"{Runtime.RuntimeVariable}.SetVariable(\"@bar\", null);",
            $"{Runtime.RuntimeVariable}.SetVariable(\"@biz\", null);"
        })]
        public void TestVarDeclaration(string code, string[] expectedCode)
        {
            SageParser parser = GetParserInAmpMode(code);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCode, parser.varDeclaration(), transpiler.StatementVisitor);
        }

        [Test]
        [TestCase("%%[ SET @FOO = 'bar' ]%%", new[]
        {
            $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", \"bar\");",
        })]
        public void TestAmpBlock(string code, string[] expectedCode)
        {
            SageParser parser = GetParserInContentMode(code);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCode, parser.contentBlock(), transpiler.BlockVisitor);
        }

        [Test]
        [TestCase("This is inline HTML", new[]
        {
            $"{Runtime.RuntimeVariable}.Output(\"This is inline HTML\");"
        })]
        [TestCase("Before %%[]%% After", new[]
        {
            $"{Runtime.RuntimeVariable}.Output(\"Before \");", 
            $"{Runtime.RuntimeVariable}.Output(\" After\");"
        })]
        public void TestInlineHtml(string ampCode, string[] cSharpStatements)
        {
            SageParser parser = GetParserInContentMode(ampCode);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(cSharpStatements, parser.contentBlock(), transpiler.BlockVisitor);
        }

        [Test]
        public void TestGeneratedMethod()
        {
            SageParser parser = GetParserInContentMode("%%[VAR @FOO]%%");
            var transpiler = new CSharpTranspiler(parser);
            string methodText = transpiler.GenerateMethodFromCode("Foo").NormalizeWhitespace().ToString();
            Assert.That(methodText, Is.EqualTo(@$"public static void Foo(RuntimeContext __runtime)
{{
    RuntimeContext __runtime = new RuntimeContext();
    {Runtime.RuntimeVariable}.SetVariable(""@foo"", null);
}}"));
        }

        private static void TestAmpCodeGeneratesExpectedCSharpCode(
            string[] expectedCSharpCode,
            ParserRuleContext context,
            SageParserBaseVisitor<IEnumerable<StatementSyntax>> statementVisitor)
        {
            string[] actualCode = statementVisitor.Visit(context)
                .Select(s => s.NormalizeWhitespace().ToString())
                .ToArray();

            Assert.That(actualCode.Length, Is.EqualTo(expectedCSharpCode.Length),
                "Number of generated statements does not match the number of cSharpExpected statements");

            for (int i = 0; i < expectedCSharpCode.Length; i++)
            {
                Assert.That(actualCode[i], Is.EqualTo(expectedCSharpCode[i]));
            }
        }
    }
}
