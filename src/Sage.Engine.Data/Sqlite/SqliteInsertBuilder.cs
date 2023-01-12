// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Data.Sqlite;

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Creates a SQLite INSERT command from a given <see cref="InsertRequest"/>
    /// </summary>
    internal class SqliteInsertBuilder : SqliteQueryBuilder
    {
        private IEnumerable<ParameterizedQueryItem> _parameters;
        private SqliteInsertBuilder(SqliteConnection connection, string dataExtension) : base(connection, dataExtension)
        {
            _parameters = Enumerable.Empty<ParameterizedQueryItem>();
        }

        /// <summary>
        /// Creates a <see cref="SqliteCommand"/> from a given <see cref="InsertRequest"/>
        /// </summary>
        /// <param name="connection">The SQLite command</param>
        /// <param name="request">The Data Extension insert request</param>
        /// <returns>A SQLite command that can be executed</returns>
        public static SqliteCommand FromRequest(SqliteConnection connection, InsertRequest request)
        {
            return new SqliteInsertBuilder(connection, request.DataExtension)
                .WithParameters(ConvertToParameters(request.AttributeData, false))
                .Build();
        }

        private SqliteInsertBuilder WithParameters(IEnumerable<ParameterizedQueryItem> parameters)
        {
            this._parameters = parameters;

            return this;
        }

        protected virtual SqliteCommand Build()
        {
            SqliteCommand command = _connection.CreateCommand();

            string columnQueryPart = BuildColumns(_parameters.Select(p => p.QuotedIdentifier));
            string columnParameterQueryPart = BuildValuesAsParameters(_parameters, command);

            command.CommandText = $"INSERT INTO {_dataExtension} ({columnQueryPart}) VALUES ({columnParameterQueryPart})";

            return command;
        }

        /// <summary>
        /// Builds the query text for a VALUES portion of an insert statement
        /// </summary>
        /// <remarks>The command object is modified with parameterized values of the input</remarks>
        /// <param name="parameters">The set of query items to be inserted</param>
        /// <param name="command">The sql command to add parameterized values to</param>
        /// <returns>Query text fragment for the given input</returns>
        protected static string BuildValuesAsParameters(
            IEnumerable<ParameterizedQueryItem> parameters,
            SqliteCommand command)
        {
            foreach (ParameterizedQueryItem parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.ParameterizedName, parameter.Data);
            }

            return string.Join(",", parameters.Select(kvp => kvp.ParameterizedName));
        }
    }
}
