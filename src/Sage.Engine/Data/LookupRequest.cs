// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data
{
    /// <summary>
    /// Represents a request to lookup data from a data extension.
    /// </summary>
    /// <param name="DataExtension">The name of the data extension to request data from</param>
    /// <param name="Limit">A limit of how many records to return</param>
    /// <param name="CaseSensitive">Whether or not the constrains are case sensitive</param>
    /// <param name="ResultAttributes">The attributes to provide in the result set</param>
    /// <param name="AttributeConstraints">The set of constraints for which data to query</param>
    /// <param name="AttributeOrdering">How the results should be ordered.</param>
    public record LookupRequest(
        string DataExtension,
        int Limit,
        bool CaseSensitive,
        IEnumerable<string> ResultAttributes,
        IEnumerable<KeyValuePair<string, string>> AttributeConstraints,
        IEnumerable<KeyValuePair<string, Order>>? AttributeOrdering);
}
