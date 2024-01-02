// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using MarketingCloudIntegration.Render;
using Microsoft.Extensions.DependencyInjection;

namespace Sage.Engine.Tests.Compatibility
{
    internal static class CompatibilityDependencyInjection
    {
        public static void AddMarketingCloudRenderingService(
            this IServiceCollection services)
        {
            services.AddSingleton<IMarketingCloudRestClient, MarketingCloudRestClient>();
            services.AddSingleton<IRenderService, RenderService>();
        }
    }
}
