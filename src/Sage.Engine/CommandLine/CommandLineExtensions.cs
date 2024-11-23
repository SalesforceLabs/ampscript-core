// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sage.Engine.CommandLine;
using Sage.Engine.Compiler;
using Sage.Engine.Extensions;

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
                        o.Content = new LocalFileContent(ampscriptOption.Source.FullName, 1);
                        o.GeneratedMethodName = CompilerOptionsBuilder.BuildMethodFromFilename(ampscriptOption.Source.FullName);
                    });
                });

            commandLineServices.AddSingleton<LaunchCommand>();
        }
    }
}
