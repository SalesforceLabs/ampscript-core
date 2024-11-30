// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sage.Engine.Compiler;
using Sage.Engine.Handlebars;
using Sage.Engine.Runtime;

namespace Sage.Engine.DependencyInjection
{
    public static class SageDependencyInjectionExtensions
    {
        public static void AddSage(
            this IServiceCollection services,
            string[] args,
            Action<SageOptions>? options = null)
        {
            services.AddSage(options);

            IServiceCollection commandLineCollection = new ServiceCollection();
            services.AddSageLaunchCommand(commandLineCollection);
            ServiceProvider provider = commandLineCollection.BuildServiceProvider();
            provider.GetRequiredService<LaunchCommand>().Invoke(args);
        }

        public static void AddSage(
            this IServiceCollection services,
            Action<SageOptions>? options = null)
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.Configure<CompilationOptions>((compilationOptions) =>
            {
                compilationOptions.AssemblyStream = new MemoryStream();
                compilationOptions.SymbolStream = new MemoryStream();
            });
            services.AddOptions<CompilationOptions>().Configure<IOptions<SageOptions>>((compilationOptions, sageOptions) =>
            {
                if (sageOptions.Value.OutputRootPath != null)
                {
                    compilationOptions.OutputDirectory = sageOptions.Value.OutputRootPath;
                }
            });
            services.AddLocalDiskContentClient();
            services.AddScoped<RuntimeContext>();
            services.AddScoped<Renderer>();
            services.AddScoped<ICompiler, AmpscriptCompiler>();
            services.AddScoped<ICompiler, HandlebarsCompiler>();
        }
    }
}
