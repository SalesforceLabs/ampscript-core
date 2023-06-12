// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Validates tests regarding ampscript data functions
    /// </summary>
    public class ContentTests : SageTest
    {
        [Test]
        [RuntimeTest("Content")]
        public EngineTestResult TestContentFunctions(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
            Assert.That(result.Output, Is.EqualTo(test.Output));
            return result;
        }
    }
}
