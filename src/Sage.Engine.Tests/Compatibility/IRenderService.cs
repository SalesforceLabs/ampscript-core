// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace MarketingCloudIntegration.Render
{
    /// <summary>
    /// An interface to the REST API
    /// </summary>
    internal interface IRenderService
    {
        /// <summary>
        /// Renders a message for the given channel
        /// </summary>
        /// <param name="channel">SMS or PUSH</param>
        Task<RenderResponse> Render(string channel, RenderRequest request);
    }
}
