// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sage.Engine.Compiler;
using Sage.Engine.DependencyInjection;

namespace Sage.Engine.Tests
{
    /// <summary>
    /// Validates the auto-generated comparision tests
    /// </summary>
    public class LaunchCommandTests : SageTest
    {
        /// <summary>
        /// Registers a class that depends on the option and invokes it
        /// </summary>
        private class TestOption
        {
            public SageOptions SageOptions;
            public CompilationOptions CompileOptions;
            public TestOption(
                IOptions<SageOptions> sageOptions,
                IOptions<CompilationOptions> compileOptions)
            {
                SageOptions = sageOptions.Value;
                CompileOptions = compileOptions.Value;
            }
        }

        [Test]
        [TestCase(
            new string[] { "launch", "ampscript", "--source", @"c:\foo\bar.txt" }, @"c:\foo", "bar.txt"
        )]
        public void TestLaunchCommand(string[] args, string expectedWorkingDirectory, string expectedFilename)
        {
            var appServices = new ServiceCollection();
            appServices.AddSage(args);

            appServices.AddTransient<TestOption>();
            ServiceProvider services = appServices.BuildServiceProvider();

            TestOption test = services.GetRequiredService<TestOption>();

            Assert.That(test.SageOptions.WorkingPath!.FullName, Is.EqualTo(expectedWorkingDirectory));
            Assert.That(test.CompileOptions.InputFile.Name, Is.EqualTo(expectedFilename));
        }
    }
}
