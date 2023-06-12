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
    /// Adds a source for "packagejson" that allows running a package manager json file
    /// </summary>
    /// <example>
    /// packagejson --source c:\code\my_package.json --sourceId 12345
    /// </example>
    public class PackageJsonCommand : Command
    {
        public PackageJsonCommand(
            CommandHandler<JsonPackageOptions> commandHandler) : base("packagejson",
            "Specifies a JSON file exported from Package Manager")
        {
            AddOption(SharedCommandLineOptions.Source);
            AddOption(new Option<string>("--sourceId", "The identifier in the selected entities to render"));

            Handler = CommandHandler.Create((FileInfo source, string sourceId) =>
            {
                var option = new JsonPackageOptions { Source = source, SourceId = sourceId };

                commandHandler.Handle(option);
            });
        }
    }
}
