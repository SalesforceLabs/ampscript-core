// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// Holdes a parent<->child relationship for packaged entieies
    /// </summary>
    /// <param name="identifier">The identifier, such as "assets/12345"</param>
    /// <param name="relationship">Whether the relationship is required or optional</param>
    internal record Reference(string identifier, string relationship);
}
