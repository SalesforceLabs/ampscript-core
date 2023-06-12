// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Webhost.DependencyInjection
{
    /// <summary>
    /// Options to control how the package extractor works.
    ///
    /// <see cref="PackageExtractor"/>
    /// </summary>
    public class PackageExtractorOptions
    {
        /// <summary>
        /// The directory to place any assets that are extracted
        /// </summary>
        public required DirectoryInfo ContentDirectory { get; set; }

        /// <summary>
        /// The directory to place any data extension CSVs that are extracted
        /// </summary>
        public required DirectoryInfo DataExtensionDirectory { get; set; }

        /// <summary>
        /// The file to write the selected entity to
        /// </summary>
        public required FileInfo GeneratedFilename { get; set; }

        /// <summary>
        /// The entity to extract from the package
        /// </summary>
        public int AssetId { get; set; }
    }
}
