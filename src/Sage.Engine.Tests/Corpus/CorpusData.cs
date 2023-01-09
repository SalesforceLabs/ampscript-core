// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

// ReSharper disable once CheckNamespace
using System.Text.RegularExpressions;

namespace Sage.Engine.Tests
{
    /// <summary>
    /// This data structure holds information about a test in a corpus file.
    /// </summary>
    public class CorpusData
    {
        /// <summary>
        /// This is the name of the test in a file-friendly manner
        /// </summary>
        public string FileFriendlyName
        {
            get; set;
        }

        /// <summary>
        /// The original name of the test
        /// </summary>
        public string OriginalName
        {
            get; set;
        }

        /// <summary>
        /// The AMPscript code in the test
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        /// The full path to the file
        /// </summary>
        public string File
        {
            get; set;
        }

        /// <summary>
        /// The line number the tests starts on
        /// </summary>
        public int LineNumber
        {
            get; set;
        }

        /// <summary>
        /// Serialized string data
        /// </summary>
        public string? ParseTree
        {
            get; set;
        }

        /// <summary>
        /// Output of executing the code
        /// </summary>
        public string? Output
        {
            get; set;
        }

        /// <summary>
        /// Whether or not this is the special 'exception' case
        /// </summary>
        public bool Exception
        {
            get
            {
                return Output == "!";
            }
        }

        public CorpusData(string originalName, string code, string file, int lineNumber)
        {
            OriginalName = originalName;
            FileFriendlyName = RemoveInvalidFilePathCharacters(originalName, "_");
            Code = code;
            File = file;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            string parseTreeSection = string.Empty;
            string outputSection = string.Empty;

            if (!string.IsNullOrEmpty(ParseTree))
            {
                parseTreeSection = $"----------\n{ParseTree}\n";
            }

            if (!string.IsNullOrEmpty(Output))
            {
                outputSection = $"++++++++++\n{Output}\n";
            }

            return $"===========\n{OriginalName}\n===========\n{Code}\n{parseTreeSection}{outputSection}";
        }

        private static string RemoveInvalidFilePathCharacters(string filename, string replaceChar)
        {
            // Handle a few specific items for readability
            string humanReadableChanges = filename
                .Replace("<", "LESS")
                .Replace(">", "GREATER")
                .Replace("/", "FSLASH")
                .Replace("\\", "BSLASH");
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + ".";
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");

            // Replace all others with the replace character
            return r.Replace(humanReadableChanges, replaceChar);
        }
    }
}
