// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// A class that represents the options for loading a Zip package from package manager
    /// </summary>
    public class ZipPackageOptions
    {
        /// <summary>
        /// The root of the unzipped package directory
        /// </summary>
        public required DirectoryInfo PackageDirectory { get; set; }

        /// <summary>
        /// The asset ID from the selected entities to load
        /// </summary>
        public required string SourceId { get; set; }
    }
}
