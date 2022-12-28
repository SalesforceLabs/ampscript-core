// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    /// <summary>
    /// These tests validate that the ANTLR4 generated parse tree is what is expected from test files
    /// </summary>
    [TestFixture]
    public class ParserTests
    {
        [Test]
        [EngineTest("Parser")]
        public EngineParseTestResult TestParserCorpus(CorpusData test)
        {
            return TestUtils.GetParseResultFromTest(test);
        }
    }
}
