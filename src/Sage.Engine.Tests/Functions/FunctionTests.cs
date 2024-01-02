// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    using NUnit.Framework;

    public class FunctionTests : SageTest
    {
        [Test]
        [RuntimeTest("Function")]
        public EngineTestResult TestFunctionsFromScriptCode(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
            Assert.That(result.Output, Is.EqualTo(test.Output));
            return result;
        }

        [Test]
        [RuntimeTest("Function")]
        [Category("Compatibility")]
        public EngineTestResult TestFunctionsFromScriptCodeCompatibility(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
            Assert.That(result.Output, Is.EqualTo(test.Output));

            if (test.Output == "!")
            {
                Assert.Throws<AggregateException>(() =>
                    TestCompatibility(test.Code, test.SubscriberContext?.GetAttributes()).Wait()
                );
            }
            else
            {
                var compatibilityResult = TestCompatibility(test.Code, test.SubscriberContext?.GetAttributes()).Result;
                Assert.That(compatibilityResult.renderedContent?.Trim(), Is.EqualTo(test.Output));
            }

            return result;
        }
    }
}
