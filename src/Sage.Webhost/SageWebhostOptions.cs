// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Webhost
{
    /// <summary>
    /// Options that control how the webhost behaves
    /// </summary>
    public class SageWebhostOptions
    {
        /// <summary>
        /// Whether or not to show the custom excpetion page which is optimized for AMPscript failures.
        /// </summary>
        public bool CustomExceptionPage { get; set; }
    }
}
