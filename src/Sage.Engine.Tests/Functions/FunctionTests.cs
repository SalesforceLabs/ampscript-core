// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    using NUnit.Framework;

    /// <summary>
    /// Validates the auto-generated comparision tests
    /// </summary>
    [TestFixture]
    public class FunctionTests
    {
        [Test]
        [RuntimeTest("Function")]
        public EngineTestResult TestFunctionsFromScriptCode(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(test);
            Assert.That(result.Output, Is.EqualTo(test.Output));
            return result;
        }
    }
}
