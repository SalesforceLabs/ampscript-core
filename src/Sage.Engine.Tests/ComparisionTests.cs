// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Validates the auto-generated comparision tests
    /// </summary>
    public class ComparisonTests : SageTest
    {
        [Test]
        [RuntimeTest("Comparisons")]
        public EngineTestResult TestComparisons(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
            Assert.That(result.Output, Is.EqualTo(test.Output));
            return result;
        }
    }
}
