// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Data.Sqlite;

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Generates a SQLite SELECT command from the <see cref="LookupRequest"/>.
    /// </summary>
    internal class SqliteSelectBuilder : SqliteQueryBuilder
    {
        IEnumerable<QuotedIdentifier> _resultAttributes;
        IEnumerable<ParameterizedQueryItem> _attributeConstraints;
        IEnumerable<KeyValuePair<QuotedIdentifier, Order>> _ordering;
        private uint _limit = 2000;

        private SqliteSelectBuilder(SqliteConnection connection, string dataExtension) : base(connection, dataExtension)
        {
            _resultAttributes = Enumerable.Empty<QuotedIdentifier>();
            _attributeConstraints = Enumerable.Empty<ParameterizedQueryItem>();
            _ordering = Enumerable.Empty<KeyValuePair<QuotedIdentifier, Order>>();
        }

        /// <summary>
        /// Generates a SQLite command from the <see cref="LookupRequest"/>.
        /// </summary>
        /// <param name="connection">The SQLite connection</param>
        /// <param name="request">The DataExtension lookup request</param>
        /// <returns>A command that can be executed</returns>
        public static SqliteCommand FromRequest(SqliteConnection connection, LookupRequest request)
        {
            SqliteSelectBuilder selectBuilder = new SqliteSelectBuilder(connection, request.DataExtension)
                .WithLimit(request.Limit)
                .WithResultAttributes(QuoteItems(request.ResultAttributes))
                .WithConstraints(ConvertToParameters(request.AttributeConstraints, request.CaseSensitive));

            if (request.AttributeOrdering != null)
            {
                selectBuilder.WithOrdering(
                    request
                        .AttributeOrdering
                        .Select(o =>
                            new KeyValuePair<QuotedIdentifier, Order>(QuotedIdentifier.Create(o.Key), o.Value)));
            }

            return selectBuilder
                .Build();
        }

        private SqliteSelectBuilder WithLimit(int limit)
        {
            this._limit = Math.Max(Math.Min(2000, (uint)limit), 1);

            return this;
        }

        private SqliteSelectBuilder WithResultAttributes(IEnumerable<QuotedIdentifier> resultAttributes)
        {
            _resultAttributes = resultAttributes;

            return this;
        }

        private SqliteSelectBuilder WithConstraints(IEnumerable<ParameterizedQueryItem> attributeConstraints)
        {
            _attributeConstraints = attributeConstraints;

            return this;
        }

        private SqliteSelectBuilder WithOrdering(IEnumerable<KeyValuePair<QuotedIdentifier, Order>> resultOrdering)
        {
            _ordering = resultOrdering;

            return this;
        }

        protected virtual SqliteCommand Build()
        {
            SqliteCommand command = _connection.CreateCommand();

            string limitQuery = $"LIMIT {this._limit}";

            string resultPropertiesQueryPart = BuildColumns(this._resultAttributes);
            string constraintQueryPart = BuildConstraint(_attributeConstraints, command);
            string orderQueryPart = BuildOrder(_ordering);

            command.CommandText = $"SELECT {resultPropertiesQueryPart} FROM {_dataExtension} WHERE {constraintQueryPart} {orderQueryPart} {limitQuery}";

            return command;
        }

        /// <summary>
        /// Builds the query text of a WHERE statement with parameterized values
        /// </summary>
        /// <remarks>The command object is modified with parameterized values of the input</remarks>
        /// <param name="constraints">The set of query parts that need to be added to the WHERE statement</param>
        /// <param name="command">The command to add parameterized queries</param>
        /// <returns>Query text for the given input.</returns>
        protected static string BuildConstraint(
            IEnumerable<ParameterizedQueryItem> constraints,
            SqliteCommand command)
        {
            foreach (var constraint in constraints)
            {
                command.Parameters.AddWithValue(constraint.ParameterizedName, constraint.Data);
            }

            return string.Join(" AND ", constraints.Select(c => c.ToString()));
        }
    }
}
