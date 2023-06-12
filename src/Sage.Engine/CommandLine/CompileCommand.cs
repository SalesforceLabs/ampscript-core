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

namespace Sage.Engine
{
    internal class CompileCommand : Command
    {
        public CompileCommand() : base("compile", "Compile and output the results")
        {
            /*
            this.AddOption(CommandLine.OutputDirectory);
            this.AddOption(CommandLine.DebugOption);

            this.AddCommand(new AmpscriptCommand(null));
            this.AddCommand(new PackageJsonCommand(null));
            this.AddCommand(new PackageZipCommand());
            */
        }
    }
}
