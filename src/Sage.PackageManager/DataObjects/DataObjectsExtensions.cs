// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    internal static class DataObjectsExtensions
    {
        internal static bool IsBlock(this AssetTypeId assetId)
        {
            switch (assetId)
            {
                case AssetTypeId.FreeformBlock:
                case AssetTypeId.TextBlock:
                case AssetTypeId.HtmlBlock:
                case AssetTypeId.TextPlusImageBlock:
                case AssetTypeId.ImageBlock:
                case AssetTypeId.ABTestBlock:
                case AssetTypeId.DynamicBlock:
                case AssetTypeId.StylingBlock:
                case AssetTypeId.EinsteinContentBlock:
                case AssetTypeId.SocialShareBlock:
                case AssetTypeId.SocialFollowBlock:
                case AssetTypeId.LayoutBlock:
                    return true;
            }

            return false;
        }

        internal static bool IsImage(this AssetTypeId assetId)
        {
            if ((int)assetId >= 16 && (int)assetId <= 39)
            {
                return true;
            }

            return false;
        }

        internal static bool IsTemplate(this AssetTypeId assetId)
        {
            return (int)assetId == 4 || (int)assetId == 214;
        }
    }
}
