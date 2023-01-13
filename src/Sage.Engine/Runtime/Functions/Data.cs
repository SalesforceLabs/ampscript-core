// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using System.Runtime.CompilerServices;
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
        public object LOOKUP(object dataExtension, object returnAttribute, params object[] queryConstraints)
        {
            string dataExtensionString = ArgumentValidator.ThrowIfStringNullOrEmpty(dataExtension);
            string returnAttributeString = ArgumentValidator.ThrowIfStringNullOrEmpty(returnAttribute);

            LookupRequestBuilder lookup = new LookupRequestBuilder(dataExtensionString)
                .WithLimit(1)
                .WithReturnedProperty(returnAttributeString);
            AddConstraints(lookup, queryConstraints);

            DataTable dataTable = GetDataExtensionClient().LookupAsync(lookup.Build()).Result;

            return dataTable.Rows[0][returnAttribute.ToString() ?? string.Empty];
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
            LookupRequestBuilder lookup = new LookupRequestBuilder(ArgumentValidator.ThrowIfStringNullOrEmpty(dataExtension))
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
            LookupRequestBuilder lookup = new LookupRequestBuilder(ArgumentValidator.ThrowIfStringNullOrEmpty(dataExtension))
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
            DataTable dataTable = ArgumentValidator.ThrowIfNotDataTable(rowset);

            return dataTable.Rows.Count;
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
            DataTable dataTable = ArgumentValidator.ThrowIfNotDataTable(rowset);

            int rowOffset = SageValue.ToInt(row) - 1;
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
            DataRow dataRow = ArgumentValidator.ThrowIfNotDataRow(row);

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
                returnResult = dataRow[ArgumentValidator.ThrowIfStringNullOrEmpty(index)];
            }

            if (returnResult == null && SageValue.ToBoolean(missingIsFailure))
            {
                throw new RuntimeException("Invalid attribute name passed to the FIELD function.  No attribute exists.");
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
                throw new RuntimeException("Dynamic parameter length must have an even number of parameters", caller);
            }

            for (int i = 0; i < constraints.Length; i += 2)
            {
                object attributeName = constraints[i];
                object attributeValue = constraints[i + 1];

                lookup.WithConstraint(
                    ArgumentValidator.ThrowIfStringNullOrEmpty(attributeName),
                    ArgumentValidator.ThrowIfStringNullOrEmpty(attributeValue));
            }
        }
    }
}
