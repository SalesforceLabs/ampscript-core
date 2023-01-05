// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;
    using Compiler;

    /// <summary>
    /// These tests validate that the ANTLR4 generated parse tree is what is cSharpExpected from test files
    /// </summary>
    [TestFixture]
    public class CompilerTests
    {
        [Test]
        [TestCase("TestAssemblyGeneration.ampscript")]
        public void TestAssemblyGeneration(string sourceFile)
        {
            string pathToTest = Path.Combine(TestContext.CurrentContext.TestDirectory, "Corpus", "Compiler", sourceFile);
            string generatedExecutablePath = Path.Combine(TestContext.CurrentContext.WorkDirectory);

            var options = new Compiler.CompilationOptions(pathToTest, generatedExecutablePath, OptimizationLevel.Debug);

            CompileResult result = CSharpCompiler.GenerateAssemblyFromSource(options);

            if (!result.EmitResult.Success)
            {
                Assert.Fail(string.Join("\n", result.EmitResult.Diagnostics.Select(d => d.ToString())));
            }
            Assert.That(File.Exists(options.OutputAssemblyFullPath));
            Assert.That(File.Exists(options.OutputSymbolsFullPath));
            Assert.IsNotNull(result.Assembly);

            var context = new RuntimeContext();
            object[] variables = new object[] { context };
            object? results = result.Assembly
                ?.GetType("Sage.Engine.Runtime.AmpProgram")
                ?.GetMethod(options.BaseName)
                ?.Invoke(null, variables);
            Assert.That(context.FlushOutputStream(), Is.EqualTo("Hello  World"));
        }
    }
}
