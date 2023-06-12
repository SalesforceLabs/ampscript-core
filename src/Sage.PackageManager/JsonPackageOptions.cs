// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// A class that represents the options for loading a json package from package manager
    /// </summary>
    public class JsonPackageOptions
    {
        /// <summary>
        /// The JSON file to load
        /// </summary>
        public required FileInfo Source { get; set; }

        /// <summary>
        /// The selected entity to load
        /// </summary>
        public required string SourceId { get; set; }
    }
}
