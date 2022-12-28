// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Sage.Engine.Tests
{
    /// <summary>
    /// Corpus files allow quickly writing many tests in a single text file.
    /// 
    /// They follow the following format:
    /// 
    /// ===
    /// TEST NAME
    /// ===
    /// %%[ Test Code %%]
    /// ---
    /// Expected parse tree, emitted in a lisp-like output
    /// ++++
    /// Expected engine output after executing code
    /// 
    /// 
    /// This may be repeated as many times as you wish for as many tests as you want to write in the file.
    /// 
    /// You may omit either the parse tree or engine output section if you do not want to validate that.
    /// 
    /// Transpiler output may also contain a "!", which means runtime exception.  You may not write a test that expects a single "!" as the output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EngineTestAttribute : Attribute, ITestBuilder
    {
        readonly string _baseDirectory;

        public EngineTestAttribute(string baseDirectory)
        {
            this._baseDirectory = baseDirectory;
        }
        public static string RemoveInvalidFilePathCharacters(string filename, string replaceChar)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(filename, replaceChar);
        }

        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
        {
            foreach (CorpusData corpusData in CorpusLoader.LoadFromDirectory(this._baseDirectory))
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(corpusData.File);

                string convertedCorpusTestName = RemoveInvalidFilePathCharacters(corpusData.Name, "_").Replace(".", "_");

                string testName = $"{method.Name}.{this._baseDirectory}.{filenameWithoutExtension}.{convertedCorpusTestName}";
                var expected = new EngineParseTestResult(corpusData.ParseTree);

                // https://github.com/nunit/nunit3-vs-adapter/issues/735
                var testData = new TestCaseData(new object?[] { corpusData }) { ExpectedResult = expected, TestName = testName };
                testData.SetProperty("_LineNumber", corpusData.LineNumber);
                testData.SetProperty("_CodeFilePath", corpusData.File);

                yield return new NUnitTestCaseBuilder()
                    .BuildTestMethod(method, suite, new TestCaseParameters(testData));
            }
        }
    }
}
