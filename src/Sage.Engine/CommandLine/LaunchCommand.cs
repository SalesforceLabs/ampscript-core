// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Engine.CommandLine;
using static Sage.Engine.AmpscriptCommand;

namespace Sage.Engine
{
    public class LaunchCommand : Command
    {
        public LaunchCommand(
            IEnumerable<ISageCompilationSource> compilationSources) : base("launch", "Render and output the results")
        {
            this.AddOption(SharedCommandLineOptions.OutputDirectory);
            this.AddOption(SharedCommandLineOptions.DebugOption);

            foreach (ISageCompilationSource source in compilationSources)
            {
                this.AddCommand(source.Command);
            }
        }
    }
}
