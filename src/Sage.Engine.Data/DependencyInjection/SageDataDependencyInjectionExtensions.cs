// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sage.Engine.Data.Sqlite;

namespace Sage.Engine.Data.DependencyInjection
{
    public static class SageDataDependencyInjectionExtensions
    {
        public static void AddInMemoryDataExtensions(
            this IServiceCollection services,
            Action<SageInMemoryDataOption>? options = null)
        {
            services.Configure<SageInMemoryDataOption>(inMemoryOptions =>
            {
                inMemoryOptions.ConnectionString = "Data Source=:memory:";
            });

            services.AddSingleton<IConfigureOptions<SageInMemoryDataOption>, ConfigureSageInMemoryDataOption>();

            if (options != null)
            {
                services.Configure(options);
            }

            services.AddScoped<IDataExtensionClient, SqliteDataExtensionClient>();
        }
    }
}
