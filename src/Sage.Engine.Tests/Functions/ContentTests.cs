// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests.Functions
{
    [TestFixture]
    internal class ContentTests : FunctionTestBase
    {
        [Test]
        [TestCase("%%=ADD(1,2)=%%", "3")]
        public void TestTreatAsContentNoOutputDirectory(string input, string expected)
        {
            var options = TestCompilationOptions();
            options.OutputDirectory = new DirectoryInfo(Path.Combine(options.OutputDirectory.FullName, "doesnotexist"));
            var context = new RuntimeContext(_serviceProvider, options);

            string results = context.TREATASCONTENT(input);
            Assert.That(results, Is.EqualTo(expected));
        }
    }
}
