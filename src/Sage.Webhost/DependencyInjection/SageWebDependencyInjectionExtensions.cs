// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.Extensions.Options;
using Sage.Engine;
using Sage.Engine.Compiler;
using Sage.Engine.Data.Sqlite;
using Sage.Engine.DependencyInjection;
using Sage.Engine.Extensions;
using Sage.PackageManager;
using Sage.PackageManager.DependencyInjection;

namespace Sage.Webhost.DependencyInjection
{
    public static class SageWebDependencyInjectionExtensions
    {
        /// <summary>
        /// In here because trying to keep PackageManager independent of Sage.Engine
        /// </summary>
        public static void AddSagePackageManagerCommandLine(
            this IServiceCollection appServices,
            IServiceCollection commandLineServices)
        {
            commandLineServices.AddSageCompilationSource<JsonPackageOptions, PackageJsonCommand>(
                packageJsonOption =>
                {
                    string generatedFilename = Path.GetFileNameWithoutExtension(packageJsonOption.Source.Name) +
                                               ".generated.ampscript";

                    appServices.AddJsonPackageLoader(o =>
                    {
                        o.Source = packageJsonOption.Source;
                        o.SourceId = packageJsonOption.SourceId;
                    });

                    if (packageJsonOption.Source.Directory == null)
                    {
                        throw new ArgumentNullException("source",
                            $"Could not identify the directory for the JSON package file {packageJsonOption.Source.FullName}");
                    }

                    var generatedFile =
                        new FileInfo(Path.Combine(packageJsonOption.Source.Directory.FullName, generatedFilename));
                    appServices.Configure<SageOptions>(o =>
                    {
                        o.WorkingPath = packageJsonOption.Source.Directory;
                        o.OutputRootPath = packageJsonOption.Source.Directory.AppendDirectory("out");
                    });
                    appServices.Configure<CompilationOptions>(o =>
                    {
                        o.GeneratedMethodName = CompilerOptionsBuilder.BuildMethodFromFilename(generatedFilename);
                        o.InputName = generatedFile.Name;
                        o.InputFile = generatedFile;
                    });
                    appServices.AddOptions<PackageExtractorOptions>()
                        .Configure<IOptions<SageInMemoryDataOption>, IOptions<ContentOptions>>(
                            (jsonPackageExtractorOptions, dataExtensionOptions, contentOptions) =>
                            {
                                jsonPackageExtractorOptions.ContentDirectory = contentOptions.Value.InputDirectory;
                                jsonPackageExtractorOptions.DataExtensionDirectory =
                                    dataExtensionOptions.Value.DataExtensionDirectory;
                                jsonPackageExtractorOptions.AssetId = int.Parse(packageJsonOption.SourceId);
                                jsonPackageExtractorOptions.GeneratedFilename = generatedFile;
                            });

                    appServices.AddTransient<IPackageLoader, JsonPackageLoader>();
                    appServices.AddTransient<PackageExtractor>();
                });

            commandLineServices.AddSageCompilationSource<ZipPackageOptions, PackageZipCommand>(
                zipPackageOptions =>
                {
                    string generatedFilename = zipPackageOptions.SourceId + ".generated.ampscript";

                    appServices.AddZipPackageLoader(o =>
                    {
                        o.PackageDirectory = zipPackageOptions.PackageDirectory;
                        o.SourceId = zipPackageOptions.SourceId;
                    });

                    var generatedFile =
                        new FileInfo(Path.Combine(zipPackageOptions.PackageDirectory.FullName, generatedFilename));
                    appServices.Configure<SageOptions>(o =>
                    {
                        o.WorkingPath = zipPackageOptions.PackageDirectory;
                        o.OutputRootPath = zipPackageOptions.PackageDirectory.AppendDirectory("out");
                    });
                    appServices.Configure<CompilationOptions>(o =>
                    {
                        o.GeneratedMethodName = CompilerOptionsBuilder.BuildMethodFromFilename(generatedFilename);
                        o.InputName = generatedFile.Name;
                        o.InputFile = generatedFile;
                    });
                    appServices.Configure<ContentOptions>(contentOptions =>
                    {
                        contentOptions.InputDirectory = zipPackageOptions.PackageDirectory.AppendDirectory("entities")
                            .AppendDirectory("assets");
                    });
                    appServices.AddOptions<PackageExtractorOptions>()
                        .Configure<IOptions<SageInMemoryDataOption>, IOptions<ContentOptions>>(
                            (jsonPackageExtractorOptions, dataExtensionOptions, contentOptions) =>
                            {
                                jsonPackageExtractorOptions.ContentDirectory = contentOptions.Value.InputDirectory;
                                jsonPackageExtractorOptions.DataExtensionDirectory =
                                    dataExtensionOptions.Value.DataExtensionDirectory;
                                jsonPackageExtractorOptions.AssetId = int.Parse(zipPackageOptions.SourceId);
                                jsonPackageExtractorOptions.GeneratedFilename = generatedFile;
                            });

                    appServices.AddTransient<PackageExtractor>();
                });
        }

        public static void AddSageWebhost(
            this IServiceCollection services,
            string[] args,
            IConfiguration config)
        {
            IServiceCollection commandLineCollection = new ServiceCollection();
            services.AddSagePackageManagerCommandLine(commandLineCollection);
            services.AddSage();
            services.AddSageLaunchCommand(commandLineCollection);
            ServiceProvider provider = commandLineCollection.BuildServiceProvider();
            provider.GetRequiredService<LaunchCommand>().Invoke(args);

            services.AddScoped<ExceptionPageRenderer>();
            services.AddScoped<WebRequestRenderer>();
            services.Configure<SageWebhostOptions>(config.GetSection("SageWebhost"));
        }
    }
}
