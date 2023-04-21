// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// The set of entities that have been chosen to be packaged.
    ///
    /// More assets may exist in the package when dependencies are found - but this list represents the set of packages
    /// the author wanted to export.
    /// </summary>
    internal record SelectedEntities(int[] assets);
}
