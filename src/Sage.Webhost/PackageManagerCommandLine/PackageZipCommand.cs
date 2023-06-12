// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Sage.PackageManager;

namespace Sage.Engine
{
    /// <summary>
    /// Adds a source for "packagezip" files from the command line
    /// </summary>
    /// <example>
    /// packagezip --packageDir c:\code\my_package --sourceId 12345
    /// </example>
    public class PackageZipCommand : Command
    {
        public PackageZipCommand(
            CommandHandler<ZipPackageOptions> commandHandler) : base("packagezip",
            "Launch from a package manager zip file that has been unzipped on disk")
        {
            AddOption(new Option<DirectoryInfo>("--packageDir",
                "The directory of an unzipped package manager package"));
            AddOption(new Option<string>("--sourceId", "The asset identifier to be rendered"));

            Handler = CommandHandler.Create((DirectoryInfo packageDir, string sourceId) =>
            {
                var option = new ZipPackageOptions { PackageDirectory = packageDir, SourceId = sourceId };

                commandHandler.Handle(option);
            });
        }
    }
}
