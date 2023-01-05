// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

// ReSharper disable once CheckNamespace
namespace Sage.Engine.Tests
{
    /// <summary>
    /// This data structure holds information about a test in a corpus file.
    /// </summary>
    public class CorpusData
    {
        public string Name
        {
            get; set;
        }

        public string Code
        {
            get; set;
        }

        public string File
        {
            get; set;
        }
        public int LineNumber
        {
            get; set;
        }

        public string? ParseTree
        {
            get; set;
        }

        public string? Output
        {
            get; set;
        }

        public bool Exception
        {
            get
            {
                return Output == "!";
            }
        }

        public CorpusData(string name, string code, string file, int lineNumber)
        {
            Name = name;
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

            if (!string.IsNullOrEmpty(outputSection))
            {
                outputSection = $"++++++++++\n{Output}\n";
            }

            return $"===========\n{Name}\n===========\n{Code}\n{parseTreeSection}{outputSection}";
        }
    }
}
