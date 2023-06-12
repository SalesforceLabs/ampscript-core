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
    public class SharedCommandLineOptions
    {
        public static readonly Option<FileInfo> Source = new Option<FileInfo>(name: "--source", description: "Path to the source file").LegalFilePathsOnly();
        public static readonly Option<DirectoryInfo> OutputDirectory = new(new[] { "--out", "-o" }, "The path to the output directory");
        public static readonly Option<bool> DebugOption = new(new[] { "--debug", "-d" }, "Whether or not to build debug information and debug the output");
    }
}
