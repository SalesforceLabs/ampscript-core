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
        public enum EntityType
        {
            Asset
        }

        public readonly Option<FileInfo> Source = new Option<FileInfo>(name: "--source", description: "Path to the source file").LegalFilePathsOnly();
        public readonly Option<EntityType> SourceType = new(new[] { "--type", "-t" }, "The type of Package Manager entity that is referenced");
        public readonly Option<string> SourceId = new(new[] { "--sourceId" }, "The Package Manager identifier to be debugged");

        public readonly Option<DirectoryInfo> PackageDirectory = new(new[] { "--packageDir" }, "The path to the directory of the unzipped Package Manager package");
        public readonly Option<DirectoryInfo> OutputDirectory = new(new[] { "--out", "-o" }, "The path to the output directory");

        public readonly Option<bool> DebugOption = new(new[] { "--debug", "-d" }, "Whether or not to build debug information and debug the output");

        public readonly Command LaunchCommand; 
        public readonly Command CompileCommand;

        public readonly Command PackageJson;
        public readonly Command PackageZip;
        public readonly Command Ampscript;

        public CommandLine()
        {
            PackageJson = new("packagejson", description: "Launch from a JSON file exported by Package Manager")
            {
                Source,
                SourceType,
                SourceId
            };

            PackageZip = new("packagezip", description: "Launch from a JSON file exported by Package Manager")
            {
                PackageDirectory,
                SourceType,
                SourceId
            };

            Ampscript = new("ampscript", description: "Launch from a standalone AMPscript file")
            {
                Source
            };

            LaunchCommand = new("launch", description: "Launch AMPScript and output the results")
            {
                PackageJson,
                PackageZip,
                Ampscript,
                DebugOption,
                OutputDirectory
            };

            CompileCommand = new("compile", description: "Compile AMPScript and output any errors")
            {
                PackageJson,
                PackageZip,
                Ampscript,
                DebugOption,
                OutputDirectory
            };
        }
    }
}
