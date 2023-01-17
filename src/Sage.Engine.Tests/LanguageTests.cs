// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;
using Sage.Engine.Runtime;

namespace Sage.Engine.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;
    using Compiler;

    /// <summary>
    /// Basic tests for the language features and runtime
    /// </summary>
    [TestFixture]
    public class LanguageTests
    {
        [Test]
        [RuntimeTest("Language")]
        public EngineTestResult TestLanguage(CorpusData test)
        {
            try
            {
                var result = TestUtils.GetOutputFromTest(test);
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