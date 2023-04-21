// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// Represents a placeholder for where content blocks may exist within the content.
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/slots.html"/>
    internal record Slot(
        string content,
        Dictionary<string, Block>? blocks,
        Dictionary<string, Slot>? slots) : Placeholder(content, blocks, slots);
}
