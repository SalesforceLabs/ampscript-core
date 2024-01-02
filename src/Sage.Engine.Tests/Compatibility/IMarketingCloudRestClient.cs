// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Compatibility
{
    /// <summary>
    /// An interface to the Marketing Cloud REST API
    /// </summary>
    internal interface IMarketingCloudRestClient
    {
        /// <summary>
        /// Sends a REST request using an exponential backoff
        /// </summary>
        HttpResponseMessage SendWithRetryBackoff(HttpRequestMessage message);

        /// <summary>
        /// Manually refreshes the auth token.  Used for testing - should not be necessary for use of the interface
        /// </summary>
        Task RefreshToken();
    }
}
