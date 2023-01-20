// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Content
{
    public static class ContentClientFactory
    {
        /// <summary>
        /// Creates a data extension client with an in-memory SQLite table as the backing store.
        ///
        /// Any changes or updates made to SQLite are lost when the connection is severed.
        /// </summary>
        public static IContentClient CreateLocalDiskContentClient(string contentDirectory)
        {
            return new LocalDiskContentClient(contentDirectory);
        }
    }
}
