// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
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
    /// The output string may also contain a single "!", which means runtime exception.  You may not write a test that expects a single "!" as the output.
    /// </summary>
    public class CorpusLoader
    {
        public static IEnumerable<CorpusData> LoadFromFile(string filename)
        {
            return GetTestsFromFileUsingHandmadeParser(filename);
        }

        public static IEnumerable<CorpusData> LoadFromDirectory(string directory)
        {
            foreach (var filename in Directory.GetFiles(Path.Combine("corpus", directory)))
            {
                foreach (CorpusData data in CorpusLoader.LoadFromFile(Path.GetFullPath(filename)))
                {
                    yield return data;
                }
            }
        }

        public static string RemoveInvalidFilePathCharacters(string filename, string replaceChar)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + ".";
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(filename, replaceChar);
        }

        private static IEnumerable<CorpusData> GetTestsFromFileUsingHandmadeParser(string file)
        {
            var testsFromFile = new List<CorpusData>();

            var parseTree = new StringBuilder();
            var code = new StringBuilder();
            var programOutput = new StringBuilder();
            bool inParseTreeBlock = false;
            bool inTestNameBlock = false;
            bool inProgramOutputBlock = false;
            bool inCodeBlock = false;
            int lineStart = 0;
            string expectedOutput = "";
            string testName = "";

            string[] allLines = File.ReadAllLines(file);
            for (int sourceLineOffset = 0; sourceLineOffset < allLines.Length; sourceLineOffset++)
            {
                string line = allLines[sourceLineOffset];
                if (line.StartsWith("==="))
                {
                    if (code.Length > 0)
                    {
                        expectedOutput = programOutput.ToString().Trim();


                        testsFromFile.Add(new CorpusData(RemoveInvalidFilePathCharacters(testName, "_"), code.ToString().Trim(), file, lineStart)
                        {
                            Output = expectedOutput,
                            ParseTree = parseTree.ToString().Trim()
                        });

                        parseTree = new StringBuilder();
                        code = new StringBuilder();
                        programOutput = new StringBuilder();
                        inParseTreeBlock = false;
                        inTestNameBlock = false;
                        inProgramOutputBlock = false;
                        inCodeBlock = false;
                        expectedOutput = "";
                        testName = "";
                    }
                    lineStart = sourceLineOffset;

                    if (string.IsNullOrEmpty(testName))
                    {
                        inTestNameBlock = true;
                    }
                    else
                    {
                        inTestNameBlock = false;
                        inCodeBlock = true;
                    }
                    continue;
                }
                if (line.StartsWith("---"))
                {
                    inParseTreeBlock = true;
                    inProgramOutputBlock = false;
                    inCodeBlock = false;
                    continue;
                }
                if (line.StartsWith("+++"))
                {
                    inProgramOutputBlock = true;
                    inParseTreeBlock = false;
                    inCodeBlock = false;
                    continue;
                }
                if (inTestNameBlock)
                {
                    testName = RemoveInvalidFilePathCharacters(line, "_");
                }
                else if (inParseTreeBlock)
                {
                    parseTree.AppendLine(line);
                }
                else if (inProgramOutputBlock)
                {
                    programOutput.AppendLine(line);
                }
                else if (inCodeBlock)
                {
                    code.AppendLine(line);
                }
            }

            expectedOutput = programOutput.ToString().Trim();
            testsFromFile.Add(new CorpusData(testName, code.ToString().Trim(), file, lineStart)
            {
                Output = expectedOutput,
                ParseTree = parseTree.ToString().Trim()
            });

            return testsFromFile;
        }
    }
}
