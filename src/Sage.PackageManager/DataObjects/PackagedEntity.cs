// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// When PackageManager packages an asset, it also provides information about the originating context.
    /// </summary>
    internal record PackagedEntity(PackagedEntity.Issues[]? issues, string? originID, string? originEID)
    {
        internal record Issues(string type, string level, string sourceJobType, string issue);
    }
}
