// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Basic tests for the language features and runtime
    /// </summary>
    public class HandlebarTests : SageTest
    {
        [Test]
        [RuntimeTest("Handlebars")]
        public EngineTestResult TestHandlebars(CorpusData test)
        {
            try
            {
                var result = TestUtils.GetOutputFromHandlebarTest(_serviceProvider, test);
                Assert.That(result.Output, Is.EqualTo(test.Output));
                return result;
            }
            catch (CompileCodeException e)
            {
                Assert.Fail(e.Message);
                return new EngineTestResult("!");
            }
        }
    }
}
