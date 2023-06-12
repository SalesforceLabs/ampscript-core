// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.NamingConventionBinder;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sage.Engine.CommandLine;
using Sage.Engine.Compiler;
using Sage.Engine.Extensions;
using static Sage.Engine.AmpscriptCommand;

namespace Sage.Engine
{
    public static class CommandLineExtensions
    {
        public static void AddSageCompilationSource<OptionsT, CommandT>(
            this IServiceCollection commandLineServices,
            Action<OptionsT> commandHandler) where CommandT : Command
        {
            commandLineServices.TryAddSingleton<CommandT>();
            commandLineServices.TryAddEnumerable(ServiceDescriptor.Singleton<ISageCompilationSource, SageCompilationSource<CommandT>>());

            commandLineServices.TryAddSingleton(new CommandHandler<OptionsT>(commandHandler));
        }

        public static void AddSageLaunchCommand(
            this IServiceCollection appServices,
            IServiceCollection commandLineServices)
        {
            commandLineServices.AddSageCompilationSource<AmpscriptOption, AmpscriptCommand>(
                ampscriptOption =>
                {
                    appServices.Configure<SageOptions>(o =>
                    {
                        if (ampscriptOption.Source.Directory == null) throw new ArgumentNullException(nameof(ampscriptOption.Source.Directory));

                        o.WorkingPath = ampscriptOption.Source.Directory;
                        o.OutputRootPath = ampscriptOption.Source.Directory.AppendDirectory("out");
                    });

                    appServices.Configure<CompilationOptions>((o) =>
                    {
                        o.InputFile = ampscriptOption.Source;
                    });
                });

            commandLineServices.AddSingleton<LaunchCommand>();
        }
    }
}
