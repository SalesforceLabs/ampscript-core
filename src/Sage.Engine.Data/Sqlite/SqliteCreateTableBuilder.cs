// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Data.Sqlite;

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Creates a SQLite CREATE TABLE command from a given <see cref="InsertRequest"/>
    /// </summary>
    internal class SqliteCreateTableBuilder : SqliteQueryBuilder
    {
        private IEnumerable<KeyValuePair<QuotedIdentifier, SqliteType>> _columns;

        private SqliteCreateTableBuilder(SqliteConnection connection, string dataExtension) : base(connection, dataExtension)
        {
            _columns = Enumerable.Empty<KeyValuePair<QuotedIdentifier, SqliteType>>();
        }

        private SqliteCreateTableBuilder WithColumns(IEnumerable<KeyValuePair<QuotedIdentifier, SqliteType>> columns)
        {
            this._columns = columns;

            return this;
        }

        public static SqliteCommand FromColumns(SqliteConnection connection, string dataExtension, IEnumerable<string> columns)
        {
            return new SqliteCreateTableBuilder(connection, dataExtension)
                .WithColumns(columns.Select(c =>
                    new KeyValuePair<QuotedIdentifier, SqliteType>(
                        QuotedIdentifier.Create(c),
                        SqliteType.Text)))
                .Build();
        }

        protected virtual SqliteCommand Build()
        {
            SqliteCommand command = _connection.CreateCommand();

            string columnQueryPart = BuildSchema(_columns);

            command.CommandText = $"CREATE TABLE {_dataExtension} ({columnQueryPart})";

            return command;
        }

        /// <summary>
        /// Returns the SQL SCHEMA text for the given input
        /// </summary>
        /// <param name="columns">The set of columns and types for the schema</param>
        /// <returns>The SQL text for the schema part of a CREATE TABLE statement</returns>
        protected static string BuildSchema(IEnumerable<KeyValuePair<QuotedIdentifier, SqliteType>> columns)
        {
            return string.Join(",", columns.Select(c => $"{c.Key} {c.Value}"));
        }
    }
}
