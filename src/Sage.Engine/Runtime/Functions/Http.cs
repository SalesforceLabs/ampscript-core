// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Used to hook into the link tracking system.
        /// </summary>
        /// <remarks>There is no current support to wrap links, so this just returns the raw link</remarks>
        public string REDIRECTTO(object? url)
        {
            return url?.ToString() ?? string.Empty;
        }
    }
}
