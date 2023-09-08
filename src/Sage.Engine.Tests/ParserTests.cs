// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// These tests validate that the ANTLR4 generated parse tree is what is expected from test files
    /// </summary>
    [TestFixture]
    public class ParserTests : SageTest
    {
        [Test]
        [ParserTest("Parser")]
        public ParserTestResult TestParserCorpus(CorpusData test)
        {
            return TestUtils.GetParseResultFromTest(test);
        }

        [Test]
        [TestCase("Parser")]
        [Ignore("Only run manually to rebuild corpus files")]
        public void RebuildTestCorpus(string folder)
        {
            foreach (KeyValuePair<string, IEnumerable<CorpusData>> corpusInFile in CorpusLoader.LoadFromDirectory(folder))
            {
                var newCorpusFileData = new List<CorpusData>();

                foreach (CorpusData corpus in corpusInFile.Value)
                {
                    CorpusData newCorpusData = corpus;
                    newCorpusData.ParseTree = TestUtils.GetParseResultFromTest(corpus).ParseTree;
                    newCorpusFileData.Add(newCorpusData);
                }

                File.WriteAllText(
                    Path.Combine(TestContext.CurrentContext.WorkDirectory, $"rebuilt_{corpusInFile.Key}"),
                    string.Join('\n', newCorpusFileData));
            }
        }
    }
}
