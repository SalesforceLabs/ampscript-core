// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine
{
    public class SageOptions
    {
        /// <summary>
        /// The command line arguments.
        /// </summary>
        public string[]? Args { get; set; }

        public DirectoryInfo? WorkingPath { get; set; }

        /// <summary>
        /// The web root path.
        /// </summary>
        public DirectoryInfo? OutputRootPath { get; set; }
    }
}
