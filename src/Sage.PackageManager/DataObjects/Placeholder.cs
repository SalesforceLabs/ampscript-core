// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// The content builder asset model contains slots and blocks.  Any piece of content can reference a slot or block
    /// through a div tag such as the following: <div data-type="(block|slot)" data-key="id"</div>.
    ///
    /// This div is a placeholder where the contents of the rendered placeholder should be placed.
    ///
    /// This class represents the blob of JSON that this div refers to. A placeholder itself may contain slots and blocks,
    /// and thus making a tree of data.
    /// </summary>
    internal record Placeholder(
        string content,
        Dictionary<string, Block>? blocks,
        Dictionary<string, Slot>? slots);
}
