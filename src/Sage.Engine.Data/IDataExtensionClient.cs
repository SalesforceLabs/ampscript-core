// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;

namespace Sage.Engine.Data
{
    /// <summary>
    /// A extension client is an interface that supports the AMPscript data extension functions in a storage-agnostic manner.
    ///
    /// Data extension clients can be implemented locally in a SQLite database or even through the SOAP/REST APIs
    /// </summary>
    public interface IDataExtensionClient : IDisposable
    {
        /// <summary>
        /// Reads data from a data extension
        /// </summary>
        /// <returns>A data table with the results of the lookup</returns>
        Task<DataTable> LookupAsync(LookupRequest lookup);

        /// <summary>
        /// Adds data to a data extension
        /// </summary>
        Task InsertAsync(InsertRequest insert);

        /// <summary>
        /// Returns the schema of a data extension table
        /// </summary>
        Task<DataTable> GetSchemaAsync(string dataExtension);

        /// <summary>
        /// Creates the client connection
        /// </summary>
        Task ConnectAsync();
    }
}
