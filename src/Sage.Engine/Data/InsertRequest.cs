// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data
{
    /// <summary>
    /// Represents a request to insert data into the data source.
    /// </summary>
    /// <param name="DataExtension">The name of the object to add data to</param>
    /// <param name="AttributeData">The key/value parameters of data to add</param>
    public record InsertRequest(
        string DataExtension,
        IEnumerable<KeyValuePair<string, string>> AttributeData);
}
