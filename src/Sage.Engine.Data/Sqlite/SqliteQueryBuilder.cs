// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Data.Sqlite;

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// A base class to facilitate safely generated SQLite commands
    /// </summary>
    internal abstract class SqliteQueryBuilder
    {
        /// <summary>
        /// The SQLite connection
        /// </summary>
        protected SqliteConnection _connection;

        /// <summary>
        /// The name of the data extension
        /// </summary>
        protected QuotedIdentifier _dataExtension;

        protected SqliteQueryBuilder(SqliteConnection connection, string dataExtension)
        {
            _connection = connection;
            _dataExtension = QuotedIdentifier.Create(dataExtension);
        }

        /// <summary>
        /// Quotes each item in the enumerable
        /// </summary>
        protected static IEnumerable<QuotedIdentifier> QuoteItems(IEnumerable<string> items)
        {
            return items.Select(QuotedIdentifier.Create);
        }

        /// <summary>
        /// Converts the provided items to SQLite parameterized query components
        /// </summary>
        /// <remarks>This is used in WHERE or INSERT clauses that data should be parameterized.</remarks>
        /// <param name="items">Keys (column names) and Values (column values) to be parameterized</param>
        /// <param name="caseSensitive">Whether or not the keys should be treated as case-insensitive</param>
        /// <returns>An enumerable set of parameterized items.</returns>
        protected static IEnumerable<ParameterizedQueryItem> ConvertToParameters(
            IEnumerable<KeyValuePair<string, string>> items,
            bool caseSensitive)
        {
            return items.Select(i => new ParameterizedQueryItem(i.Key, i.Value, caseSensitive));
        }

        /// <summary>
        /// Returns query text of the columns
        /// </summary>
        /// <param name="columnNames">Columns to return</param>
        /// <returns>Query text for given input</returns>
        protected static string BuildColumns(IEnumerable<QuotedIdentifier> columnNames)
        {
            return string.Join(",", columnNames);
        }

        /// <summary>
        /// Builds an ORDER BY clause for the given ordering
        /// </summary>
        /// <param name="orderings">Columns and order to include</param>
        /// <returns>Query fragment with the ORDER BY statement. Empty string if no orderings are requested</returns>
        protected static string BuildOrder(IEnumerable<KeyValuePair<QuotedIdentifier, Order>> orderings)
        {
            if (orderings.Any())
            {
                return "ORDER BY " + string.Join(",", orderings.Select(o => $"{o.Key} {o.Value}"));
            }

            return "";
        }
    }
}
