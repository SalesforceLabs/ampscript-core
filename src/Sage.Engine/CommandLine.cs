// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;

namespace Sage.Engine
{
    /// <summary>
    /// Contains shared command line options regardless of host.
    ///
    /// Specific hosts will have specific handlers - for example, the webhost will map GETs to the provided source file and consoles will output to the console.
    /// </summary>
    public class CommandLine
    {
        public readonly Argument<string> sourceFile = new Argument<string>(name: "--source", description: "Path to the AMPscript program to debug").LegalFilePathsOnly();
        public readonly Option<bool> debugOption = new(new[] { "--debug", "-d" }, "Whether or not to build debug information and debug the output");

        public readonly Command LaunchCommand;
        public readonly Command CompileCommand;

        public CommandLine()
        {
            LaunchCommand = new("launch", description: "Launch AMPScript and output the results")
            {
                sourceFile,
                debugOption
            };

            CompileCommand = new("compile", description: "Compile AMPScript and output any errors")
            {
                sourceFile,
                debugOption
            };
        }
    }
}
