// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// Loads a package from package manager
    /// </summary>
    public interface IPackageLoader
    {
        /// <summary>
        /// Returns the package graph after loading from package manager
        /// </summary>
        PackageGraph Load();
    }
}
