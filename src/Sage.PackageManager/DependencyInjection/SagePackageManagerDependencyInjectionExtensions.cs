// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.DependencyInjection;

namespace Sage.PackageManager.DependencyInjection
{
    public static class SagePackageManagerDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds an IPackageLoader to the container to load zip packages
        /// </summary>
        public static void AddZipPackageLoader(
            this IServiceCollection services,
            Action<ZipPackageOptions> options)
        {
            services.Configure(options);

            services.AddTransient<IPackageLoader, ZipPackageLoader>();
        }

        /// <summary>
        /// Adds an IPackageLoader to the container that loads JSON packages
        /// </summary>
        public static void AddJsonPackageLoader(
            this IServiceCollection services,
            Action<JsonPackageOptions> options)
        {
            services.Configure(options);

            services.AddTransient<IPackageLoader, JsonPackageLoader>();
        }
    }
}
