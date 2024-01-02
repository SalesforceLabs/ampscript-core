// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.DependencyInjection;

namespace Sage.Engine.Tests.Compatibility
{
    /// <summary>
    /// Sanity tests for getting an auth token
    /// </summary>
    internal class AuthTests : SageTest
    {
        [Test]
        [Category("Compatibility")]
        public void GetAuthToken()
        {
            _serviceProvider.GetRequiredService<IMarketingCloudRestClient>().RefreshToken();
        }
    }
}
