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
    public class LanguageTests : SageTest
    {
        [Test]
        [RuntimeTest("Language")]
        public EngineTestResult TestLanguage(CorpusData test)
        {
            try
            {
                var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
                Assert.That(result.Output, Is.EqualTo(test.Output));
                return result;
            }
            catch (CompileCodeException e)
            {
                Assert.Fail(e.Message);
                return new EngineTestResult("!");
            }
        }

        [Test]
        [RuntimeTest("Language")]
        [Category("Compatibility")]
        public EngineTestResult TestLanguageCompatibility(CorpusData test)
        {
            try
            {
                var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
                Assert.That(result.Output, Is.EqualTo(test.Output));

                var compatibilityResult = TestCompatibility(test.Code, test.SubscriberContext?.GetAttributes()).Result;

                Assert.That(compatibilityResult.renderedContent?.Trim().ReplaceLineEndings("\n"), Is.EqualTo(test.Output?.ReplaceLineEndings("\n")));
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