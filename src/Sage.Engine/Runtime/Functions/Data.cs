// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Sage.Engine.Data;

// Ignore the following in this file - mostly due to enabling this codebase to adhere to these rules but AMPscript code may not.
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConstantConditionalAccessQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Returns a single value from a single attribute of a data extension
        /// </summary>
        /// <param name="dataExtension">The name of the data extension</param>
        /// <param name="returnAttribute">The name of the attribute to return</param>
        /// <param name="queryConstraints">Repeating pairs of parameters for which rows to retrieve. Must be even. i==attribute i+1==attributeValue</param>
        public object? LOOKUP(object dataExtension, object returnAttribute, params object[] queryConstraints)
        {
            string dataExtensionString = this.ThrowIfStringNullOrEmpty(dataExtension);
            string returnAttributeString = this.ThrowIfStringNullOrEmpty(returnAttribute);

            LookupRequestBuilder lookup = new LookupRequestBuilder(dataExtensionString)
                .WithLimit(1)
                .WithReturnedProperty(returnAttributeString);
            AddConstraints(lookup, queryConstraints);

            DataTable dataTable = GetDataExtensionClient().LookupAsync(lookup.Build()).Result;

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][returnAttribute.ToString() ?? string.Empty];
            }

            return null;
        }

        /// <summary>
        /// Returns a set of unordered rows (rowset) from a data extension
        /// </summary>
        /// <param name="dataExtension">The name of the data extension</param>
        /// <param name="queryConstraints">Repeating pairs of parameters for which rows to retrieve. Must be even. i==attribute i+1==attributeValue</param>
        public DataTable LOOKUPROWS(
            object dataExtension,
            params object[] queryConstraints)
        {
            LookupRequestBuilder lookup = new LookupRequestBuilder(this.ThrowIfStringNullOrEmpty(dataExtension))
                .WithLimit(2000);
            AddConstraints(lookup, queryConstraints);

            return GetDataExtensionClient().LookupAsync(lookup.Build()).Result;
        }

        /// <summary>
        /// Returns a set of unordered rows (rowset) from a data extension using a case insensitive search
        /// </summary>
        /// <param name="dataExtension">The name of the data extension</param>
        /// <param name="queryConstraints">Repeating pairs of parameters for which rows to retrieve. Must be even. i==attribute i+1==attributeValue</param>
        public DataTable LOOKUPROWSCS(
            object dataExtension,
            params object[] queryConstraints)
        {
            LookupRequestBuilder lookup = new LookupRequestBuilder(this.ThrowIfStringNullOrEmpty(dataExtension))
                .WithLimit(2000)
                .WithCaseSensitivity(true);
            AddConstraints(lookup, queryConstraints);

            return GetDataExtensionClient().LookupAsync(lookup.Build()).Result;
        }

        /// <summary>
        /// Returns the count of rows in the provided rowset
        /// </summary>
        /// <param name="rowset">The rowset to count the row results</param>
        /// <returns>The number of rows in the rowset</returns>
        public int ROWCOUNT(object rowset)
        {
            if (ArgumentValidator.IsOfType<JsonArray>(rowset, out JsonArray? jsonRowset))
            {
                return jsonRowset.Count;
            }
            else
            {
                DataTable dataTable = this.ThrowIfNotDataTable(rowset);

                return dataTable.Rows.Count;
            }
        }

        /// <summary>
        /// Returns a row from the provided rowset
        /// </summary>
        /// <param name="rowset">The rowset to get the row from</param>
        /// <param name="row">The offset within the rowset to obtain the row from. This is a 1-based index.</param>
        /// <returns>A row within the rowset at the specified offset</returns>
        public DataRow ROW(
            object rowset,
            object row)
        {
            int rowOffset = SageValue.ToInt(row) - 1;
            DataTable dataTable;
            if (ArgumentValidator.IsOfType<JsonArray>(rowset, out JsonArray? jsonRowset))
            {
                // This is quite expensive to generate a data table for the entire array each time a ROW function is called.
                // Ideally, the data table is generated one time for the given rowset and reused.
                // Maybe a nice improvement for later.
                dataTable = ConvertJsonArrayOfObjectsToDatatable(jsonRowset);
            }
            else
            {
                dataTable = this.ThrowIfNotDataTable(rowset);
            }

            if (dataTable.Rows.Count < rowOffset)
            {
                throw new RuntimeException($"Index out of bounds. Index={rowOffset} Rows={dataTable.Rows.Count}", this);
            }

            return dataTable.Rows[rowOffset];
        }

        /// <summary>
        /// Returns the value of a field from a row. If the field does not exist, a runtime error is generated.
        /// </summary>
        /// <remarks>
        /// At the moment, only a data row is supported. API objects are not supported.
        /// </remarks>
        /// <param name="row">The row which contains the field</param>
        /// <param name="index">The offset, which can be an integer or string. If an integer, it's 1-based offset. If it's a string, it's the attribute name.</param>
        public object FIELD(
            object row,
            object index)
        {
            return FIELD(row, index, 1);
        }

        /// <summary>
        /// Returns the value of a field from a row set.
        /// </summary>
        /// <remarks>
        /// At the moment, only a data row is supported. API objects are not supported.
        /// </remarks>
        /// <param name="row">The rowset which contains the field</param>
        /// <param name="index">The offset, which can be an integer or string. If an integer, it's 1-based offset.</param>
        /// <param name="missingIsFailure">If the value is missing - should this function fail? 1==runtime error, 0==return null. Default is 1</param>
        public object FIELD(
            object row,
            object index,
            object missingIsFailure)
        {
            // TODO: The input may also be an API object, but that is not supported yet, so for now it must be a DataRow.
            DataRow dataRow = this.ThrowIfNotDataRow(row);

            object? returnResult = null;

            if (SageValue.TryToInt(index, out int intResult) != SageValue.UnboxResult.Fail)
            {
                if (intResult > dataRow.Table?.Columns?.Count)
                {
                    returnResult = dataRow[intResult - 1];
                }
            }
            else
            {
                returnResult = dataRow[this.ThrowIfStringNullOrEmpty(index)];
            }

            if (returnResult == null && SageValue.ToBoolean(missingIsFailure))
            {
                throw new RuntimeException("Invalid attribute name passed to the FIELD function.  No attribute exists.", this);
            }

            return returnResult ?? string.Empty;
        }

        /// <summary>
        /// Adds dynamic length parameters as constraints.
        /// </summary>
        /// <remarks>A constraint is what ends up in the WHERE clause of the query.</remarks>
        /// <param name="lookup">The lookup to add constraints to</param>
        /// <param name="constraints">The dynamic pairs of attribute name & attribute values</param>
        private void AddConstraints(
            LookupRequestBuilder lookup,
            object[] constraints,
            [CallerMemberName] string caller = "")
        {
            if (constraints.Length % 2 != 0)
            {
                throw new RuntimeException("Dynamic parameter length must have an even number of parameters", this, caller);
            }

            for (int i = 0; i < constraints.Length; i += 2)
            {
                object attributeName = constraints[i];
                object attributeValue = constraints[i + 1];

                lookup.WithConstraint(
                    this.ThrowIfStringNullOrEmpty(attributeName, caller),
                    this.ThrowIfStringNullOrEmpty(attributeValue, caller));
            }
        }

        /// <summary>
        /// Creates a data table from the values in the JsonArray. This is so that ROW and FIELD can work as expected
        /// on data that originally came in as a JSON array.
        /// </summary>
        private DataTable ConvertJsonArrayOfObjectsToDatatable(JsonArray jsonArray)
        {
            var dataTable = new DataTable();
            if (jsonArray.Count == 0)
            {
                return dataTable;
            }

            var firstJsonObject = jsonArray[0] as JsonObject;
            if (firstJsonObject == null)
            {
                throw new InternalEngineException("No json object in the json array");
            }

            foreach (KeyValuePair<string, JsonNode?> property in firstJsonObject)
            {
                dataTable.Columns.Add(property.Key);
            }

            foreach (JsonNode? obj in jsonArray)
            {
                var jsonObject = obj as JsonObject;

                if (jsonObject == null)
                {
                    continue;
                }

                DataRow nextRow = dataTable.NewRow();
                foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
                {
                    nextRow[property.Key] = property.Value?.ToString();
                }

                dataTable.Rows.Add(nextRow);
            }

            return dataTable;
        }
    }
}
