// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sage.Engine.Content;
using Sage.Engine.Extensions;

namespace Sage.Engine.Compiler
{
    public static class ContentExtensions
    {
        public static void AddLocalDiskContentClient(this IServiceCollection services) =>
            AddLocalDiskContentClient(services, null);

        public static void AddLocalDiskContentClient(this IServiceCollection services, Action<ContentOptions>? options = null)
        {
            services.AddOptions<ContentOptions>().Configure<IOptions<SageOptions>>((contentOptions, sageOptions) =>
            {
                if (sageOptions.Value.OutputRootPath != null)
                {
                    contentOptions.OutputDirectory = sageOptions.Value.OutputRootPath;
                }

                if (sageOptions.Value.WorkingPath != null)
                {
                    contentOptions.InputDirectory = sageOptions.Value.WorkingPath.AppendDirectory("Content");
                }
            });

            services.TryAddScoped<IClassicContentClient, LocalDiskContentClient>();
            services.TryAddScoped<IContentBuilderContentClient, LocalDiskContentClient>();

            if (options != null)
            {
                services.Configure(options);
            }
        }
    }
}
