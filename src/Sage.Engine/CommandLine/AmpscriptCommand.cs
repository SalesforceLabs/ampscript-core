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
    public class AmpscriptCommand : Command
    {
        public delegate void AmpscriptCommandHandler(AmpscriptOption source);

        public AmpscriptCommand(
            CommandHandler<AmpscriptOption> commandHandler) : base("ampscript", "Specifies an ampscript file as the source")
        {
            this.AddOption(SharedCommandLineOptions.Source);

            this.Handler = CommandHandler.Create((FileInfo source) =>
            {
                var option = new AmpscriptOption() { Source = source };

                commandHandler.Handle(option);
            });
        }
    }
}
