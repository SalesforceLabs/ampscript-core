// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.DependencyInjection;
using Sage.Engine.Extensions;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

namespace Sage.Engine.Tests
{
    [TestFixture]
    public class SageTest
    {
        protected IServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void SetupSageTestContainer()
        {
            var collection = new ServiceCollection();

            collection.AddSage((sageOptions =>
            {
                sageOptions.WorkingPath = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
                sageOptions.OutputRootPath = sageOptions.WorkingPath.AppendDirectory("SageOutput");
            }));

            _serviceProvider = collection.BuildServiceProvider();
        }

        protected CompilationOptions TestCompilationOptions()
        {
            return new CompilationOptions()
            {
                InputFile = new FileInfo("TEST.ampscript"),
                InputName = "TEST",
                SymbolStream = new MemoryStream(),
                AssemblyStream = new MemoryStream(),
                OptimizationLevel = OptimizationLevel.Debug,
                GeneratedMethodName = "TEST",
                OutputDirectory = new DirectoryInfo(TestContext.CurrentContext.WorkDirectory)
            };
        }
    }
}
