// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// Assets may belong to folders, and folders are represented in the asset model through Categories.
    /// </summary>
    internal record Category(string description, string categoryType, int parentId, string name, int editable, int extendable);

    /// <summary>
    /// A category from PackageManager
    /// </summary>
    internal record PackagedCategory(PackagedEntity.Issues[]? issues, string originID, string originEID, Category data) :
            PackagedEntity(issues, originID, originEID);
}
