﻿// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{ using Antlr4.Runtime;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Parser;
    using Transpiler;

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
            var lexer = new SageLexer(stream);
            lexer.Mode(mode);
            ITokenStream tokens = new CommonTokenStream(lexer);
            return new SageParser(tokens);
        }

        [Test]
        public void TestVariableDeclaration()
        {
            SageTest.AssertEqualGeneratedCode(
                Runtime.GetInitializationStatement(),
                $"RuntimeContext {Runtime.RuntimeVariable} = new RuntimeContext();");
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
            $"#line (1, 1) - (1, 9) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@foo\", null);"
        })]
        [TestCase("VAR @FOO, @BAR, @BIZ", new[]
        {
            $"#line (1, 1) - (1, 21) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@foo\", null);",
            $"#line (1, 1) - (1, 21) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@bar\", null);",
            $"#line (1, 1) - (1, 21) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@biz\", null);"
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
        [TestCase("%%= V(@FOO) =%%", new[]
        {
            $"{Runtime.RuntimeVariable}.SetCurrentContextLineNumber(1);",
            $"#line (1, 1) - (1, 16) \"TEST.cs\"\n{Runtime.RuntimeVariable}.Output({Runtime.RuntimeVariable}.V({Runtime.RuntimeVariable}.GetVariable(\"@foo\")));",
        })]
        public void TestInlineAmpBlock(string code, string[] expectedCode)
        {
            SageParser parser = GetParserInContentMode(code);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(expectedCode, parser.contentBlock(), transpiler.BlockVisitor);
        }

        [Test]
        [TestCase("This is inline HTML", new[]
        {
            $"#line hidden\n{Runtime.RuntimeVariable}.Output(\"This is inline HTML\");"
        })]
        [TestCase("Before %%[]%% After", new[]
        {
            $"#line hidden\n{Runtime.RuntimeVariable}.Output(\"Before \");",
            $"#line hidden\n{Runtime.RuntimeVariable}.Output(\" After\");"
        })]
        public void TestInlineHtml(string ampCode, string[] cSharpStatements)
        {
            SageParser parser = GetParserInContentMode(ampCode);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(cSharpStatements, parser.contentBlock(), transpiler.BlockVisitor);
        }

        [Test]
        [TestCase("FOR @I=1 TO 10 DO VAR @VAR NEXT @I", new[]
        {
            $"#line (1, 5) - (1, 9) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@i\", SageValue.ToLong(1L));",
            $"#line (1, 13) - (1, 15) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"__for_target_@i_1\", SageValue.ToLong(10L));",
            @$"#line (1, 1) - (1, 18) ""TEST.cs""
while (SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""@i"")) <= SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""__for_target_@i_1"")))
{{
#line (1, 19) - (1, 27) ""TEST.cs""
    {Runtime.RuntimeVariable}.SetVariable(""@var"", null);
#line (1, 28) - (1, 32) ""TEST.cs""
    {Runtime.RuntimeVariable}.SetVariable(""@i"", ((long){Runtime.RuntimeVariable}.GetVariable(""@i"").Value) + 1);
#line hidden
    if (SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""@i"")) == long.MaxValue)
    {{
        break;
    }}
}}"
        })]
        [TestCase("FOR @I=10 DOWNTO 1 DO VAR @VAR NEXT @I", new[]
        {
            $"#line (1, 5) - (1, 10) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"@i\", SageValue.ToLong(10L));",
            $"#line (1, 18) - (1, 19) \"TEST.cs\"\n{Runtime.RuntimeVariable}.SetVariable(\"__for_target_@i_1\", SageValue.ToLong(1L));",
            @$"#line (1, 1) - (1, 22) ""TEST.cs""
while (SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""@i"")) >= SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""__for_target_@i_1"")))
{{
#line (1, 23) - (1, 31) ""TEST.cs""
    {Runtime.RuntimeVariable}.SetVariable(""@var"", null);
#line (1, 32) - (1, 36) ""TEST.cs""
    {Runtime.RuntimeVariable}.SetVariable(""@i"", ((long){Runtime.RuntimeVariable}.GetVariable(""@i"").Value) - 1);
#line hidden
    if (SageValue.ToLong({Runtime.RuntimeVariable}.GetVariable(""@i"")) == long.MinValue)
    {{
        break;
    }}
}}"
        })]
        public void TestForLoop(string ampCode, string[] cSharpStatements)
        {
            SageParser parser = GetParserInAmpMode(ampCode);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(cSharpStatements, parser.contentBlock(), transpiler.StatementVisitor);
        }

        [Test]
        [TestCase("IF (true) THEN ENDIF", new[] { @"#line (1, 1) - (1, 15) ""TEST.cs""
if (true)
{
}" })]
        [TestCase("IF (true) THEN ELSEIF (false) THEN ENDIF", new[] { @"#line (1, 1) - (1, 15) ""TEST.cs""
if (true)
{
}
else 
#line (1, 16) - (1, 35) ""TEST.cs""
if (false)
{
}" })]
        [TestCase("IF (true) THEN ELSEIF (false) THEN ELSE ENDIF", new[] { @"#line (1, 1) - (1, 15) ""TEST.cs""
if (true)
{
}
else 
#line (1, 16) - (1, 35) ""TEST.cs""
if (false)
{
}
#line (1, 36) - (1, 40) ""TEST.cs""
else
{
}" })]
        public void TestIfStatement(string ampCode, string[] cSharpStatements)
        {
            SageParser parser = GetParserInAmpMode(ampCode);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(cSharpStatements, parser.contentBlock(), transpiler.StatementVisitor);
        }

        [Test]
        [TestCase("%%[ SET @VAR = [Foo] ]%%", new[]
        {
            $"{Runtime.RuntimeVariable}.SetVariable(\"@var\", {Runtime.RuntimeVariable}.ATTRIBUTEVALUE(\"Foo\"));"
        })]
        [TestCase("HELLO %%Foo%%", new[]
        {
            "#line hidden\n__runtime.Output(\"HELLO \");",
            "#line hidden\n__runtime.Output(__runtime.ATTRIBUTEVALUE(\"Foo\"));"
        })]
        public void TestAttributes(string ampCode, string[] cSharpStatements)
        {
            SageParser parser = GetParserInContentMode(ampCode);
            var transpiler = new CSharpTranspiler(parser);
            TestAmpCodeGeneratesExpectedCSharpCode(cSharpStatements, parser.contentBlock(), transpiler.BlockVisitor);
        }

        [Test]
        public void TestGeneratedMethod()
        {
            var transpiler = CSharpTranspiler.CreateFromSource("%%[VAR @FOO]%%");
            
            SageTest.AssertEqualGeneratedCode(transpiler.GenerateMethodFromCode("Foo"), @$"#line hidden
public static void Foo(RuntimeContext __runtime)
{{
#line (1, 4) - (1, 12) ""TEST""
    {Runtime.RuntimeVariable}.SetVariable(""@foo"", null);
}}");
        }

        private static void TestAmpCodeGeneratesExpectedCSharpCode(
            string[] expectedCSharpCode,
            ParserRuleContext context,
            SageParserBaseVisitor<IEnumerable<StatementSyntax>> statementVisitor)
        {
            var actualCode = statementVisitor.Visit(context).ToArray();

            Assert.That(actualCode.Length, Is.EqualTo(expectedCSharpCode.Length),
                "Number of generated statements does not match the number of cSharpExpected statements");

            for (int i = 0; i < expectedCSharpCode.Length; i++)
            {
                SageTest.AssertEqualGeneratedCode(actualCode[i], expectedCSharpCode[i]);
            }
        }

        [Test]
        public void TestGenerateCompilationUnit()
        {
            var transpiler = CSharpTranspiler.CreateFromSource("%%[VAR @FOO]%%");
            SageTest.AssertEqualGeneratedCode(transpiler.GenerateCode(), @$"namespace Sage.Engine.Runtime
{{
    using System.Collections.Generic;
    using System.Text;
    using System;

    public static class AmpProgram
    {{
#line hidden
        public static void TEST(RuntimeContext __runtime)
        {{
#line (1, 4) - (1, 12) ""TEST""
            {Runtime.RuntimeVariable}.SetVariable(""@foo"", null);
        }}
    }}
}}");
        }

        /// <summary>
        /// Some expression statements do not make sense in the grammar and are ignored by the existing interpreter.
        /// To implement this, I have enabled them in the parse tree, but they are then ignored when transpiled.
        /// I probably should just update the grammar to ignore this ¯\_(ツ)_/¯
        /// </summary>
        [Test]
        [TestCase("5<5")]
        [TestCase("5||5")]
        [TestCase("@foo")]
        [TestCase("@foo='bar'")]
        [TestCase("(5<5)")]
        [TestCase("(5||5)")]
        [TestCase("(@foo)")]
        [TestCase("(@foo=('bar'))")]
        public void NonSensicalStatements(string statement)
        {
            SageParser parser = GetParserInAmpMode(statement);
            var transpiler = new CSharpTranspiler(parser);

            TestAmpCodeGeneratesExpectedCSharpCode(new string[] { }, parser.ampStatement(), transpiler.StatementVisitor);
        }
    }
}
