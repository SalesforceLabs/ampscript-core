// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// A Block is an asset block in the content builder asset model.
    ///
    /// This does not represent everything in the payload, only the relevant details to rendering content.
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/blocks.html"/>
    internal record Block(
        AssetType type,
        string content,
        Dictionary<string, Block> blocks,
        Dictionary<string, Slot> slots) : Placeholder(content, blocks, slots);
}
