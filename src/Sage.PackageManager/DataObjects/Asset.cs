// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// Represents an asset in the content builder SDK.
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/asset_type.html"/>
    internal record Asset(AssetType assetType, string name, Views views);

    /// <summary>
    /// Maps the ID number to a human readable name
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/base-asset-types.html"/>
    internal enum AssetTypeId
    {
        Asset = 1,
        File = 2,
        Block = 3,
        Template = 4,
        Message = 5,
        TemplateBasedEmail = 207,
        HtmlEmail = 208,
    }

    /// <summary>
    /// The type of asset that is being referenced
    /// </summary>
    /// <param name="id">The ID of the asset</param>
    /// <param name="name">A human readable name for this asset type</param>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/base-asset-types.html"/>
    internal record AssetType(AssetTypeId id, string name);

    /// <summary>
    /// An asset from PackageManager
    /// </summary>
    internal record PackagedAsset(PackagedEntity.Issues[]? issues, string originID, string originEID, Asset data) :
        PackagedEntity(issues, originID, originEID);
}
