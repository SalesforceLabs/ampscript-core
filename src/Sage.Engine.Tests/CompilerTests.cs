// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests
{
    using NUnit.Framework;
    using Compiler;

    /// <summary>
    /// These tests validate that the ANTLR4 generated parse tree is what is cSharpExpected from test files
    /// </summary>
    public class CompilerTests : SageTest
    {
        [Test]
        [TestCase("TestAssemblyGeneration.ampscript")]
        public void TestAssemblyGeneration(string sourceFile)
        {
            string pathToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, "Corpus", "Compiler", sourceFile);

            Compiler.CompilationOptions options = new CompilerOptionsBuilder()
                .WithInputFile(new FileInfo(pathToTest))
                .Build();

            CompileResult result = CSharpCompiler.GenerateAssemblyFromSource(options);

            if (!result.EmitResult.Success)
            {
                Assert.Fail(string.Join("\n", result.EmitResult.Diagnostics.Select(d => d.ToString())));
            }

            Assert.That(result.Assembly, Is.Not.Null);

            var context = new RuntimeContext(_serviceProvider, options);
            object[] variables = new object[] { context };
            result.Assembly
                ?.GetType("Sage.Engine.Runtime.AmpProgram")
                ?.GetMethod(options.GeneratedMethodName)
                ?.Invoke(null, variables);
            Assert.That(context.PopContext(), Is.EqualTo("Hello  World"));
        }
    }
}