// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketingCloudIntegration.Render;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.Compiler;
using Sage.Engine.DependencyInjection;
using Sage.Engine.Extensions;
using Sage.Engine.Tests.Compatibility;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

namespace Sage.Engine.Tests
{
    [TestFixture]
    public class SageTest
    {
        protected ServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void SetupSageTestContainer()
        {
            var collection = new ServiceCollection();

            collection.AddSage((sageOptions =>
            {
                sageOptions.WorkingPath = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
                sageOptions.OutputRootPath = sageOptions.WorkingPath.AppendDirectory("SageOutput");
            }));

            collection.AddMarketingCloudRenderingService();

            _serviceProvider = collection.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void TearDownContainer()
        {
            _serviceProvider.Dispose();
        }

        protected CompilationOptions TestCompilationOptions()
        {
            return new CompilationOptions()
            {
                Content = new EmbeddedContent("TEST", "TEST", "TEST.ampscript", 1, ContentType.AMPscript),
                SymbolStream = new MemoryStream(),
                AssemblyStream = new MemoryStream(),
                OptimizationLevel = OptimizationLevel.Debug,
                GeneratedMethodName = "TEST",
                OutputDirectory = new DirectoryInfo(TestContext.CurrentContext.WorkDirectory)
            };
        }

        /// <summary>
        /// Encapsulates how the test validates the code is identical between generated and expected code.
        /// </summary>
        public static void AssertEqualGeneratedCode<TNode>(TNode actual, string expected, bool withTrivia = true) where TNode : SyntaxNode
        {
            if (!withTrivia)
            {
                actual = actual.WithoutTrivia();
            }
            Assert.That(actual.NormalizeWhitespace(eol: "\n").ToFullString(), Is.EqualTo(expected.ReplaceLineEndings("\n")));
        }

        public async Task<RenderResponse> TestCompatibility(string input, Dictionary<string, string>? attributes = null)
        {
            var request = new RenderRequest()
            {
                attributes = attributes,
                context = "Preview",
                content = input,
                recipient = new Recipient
                {
                    contactKey = "sample@example.com"
                }
            };
            return await _serviceProvider.GetRequiredService<IRenderService>().Render("SMS", request);
        }
    }
}
