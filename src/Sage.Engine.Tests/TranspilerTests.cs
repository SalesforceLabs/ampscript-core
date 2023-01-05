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
            var transpiler = new CSharpTranspiler();

            LiteralExpressionSyntax transpiledString = transpiler.StringVisitor.Visit(parser.@string());
            Assert.That(transpiledString.Token.Value?.ToString(),  Is.EqualTo(expected));
        }

        [Test]
        [TestCase("SET @FOO = 1", $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", 1L);")]
        [TestCase("SET @BAR = 'Foo'", $"{Runtime.RuntimeVariable}.SetVariable(\"@bar\", \"Foo\");")]
        [TestCase("SET @BAR = @FOO", $"{Runtime.RuntimeVariable}.SetVariable(\"@bar\", {Runtime.RuntimeVariable}.GetVariable(\"@foo\"));")]
        public void TestSet(string code, string expected)
        {
            SageParser parser = GetParserInAmpMode(code);

            TestAmpCodeGeneratesExpectedCSharpCode(new[] { expected }, parser.setVariable());
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
            TestAmpCodeGeneratesExpectedCSharpCode(new[] { cSharpCodeExpected }, parser.setVariable());
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
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCode, parser.varDeclaration());
        }

        [Test]
        [TestCase("%%[ SET @FOO = 'bar' ]%%", new[]
        {
            $"{Runtime.RuntimeVariable}.SetVariable(\"@foo\", \"bar\");",
        })]
        public void TestAmpBlock(string code, string[] expectedCode)
        {
            SageParser parser = GetParserInContentMode(code);
            var transpiler = new CSharpTranspiler();
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCode, parser.contentBlock(), transpiler.BlockVisitor);
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

        private static void TestAmpCodeGeneratesExpectedCSharpCode(string[] expectedCSharpCode, ParserRuleContext context)
        {
            var transpiler = new CSharpTranspiler();
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCSharpCode, context, transpiler.StatementVisitor);
        }
    }
}
