// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Implements the <seealso cref="IDataExtensionClient"/> interface using a SQLite database
    /// </summary>
    /// <remarks>
    /// The current implementation automatically loads underlying data extensions from CSVs on-demand.
    ///
    /// Once the CSV files are loaded, they are not referenced again and all the rest of the data manipulation
    /// happens in the SQLite database.
    /// </remarks>
    internal class SqliteDataExtensionClient : IDataExtensionClient
    {
        private readonly SageInMemoryDataOption _inMemoryOptions;

        /// <summary>
        /// The set of tables that have been loaded from CSV into memory.
        /// The key is the QuotedIdentifier.
        /// </summary>
        private readonly HashSet<QuotedIdentifier> _loadedTables = new();

        /// <summary>
        /// The underlying SQLite connection.
        /// </summary>
        internal SqliteConnection Connection
        {
            get;
        }

        public SqliteDataExtensionClient(IOptions<SageInMemoryDataOption> inMemoryOptions)
        {
            _inMemoryOptions = inMemoryOptions.Value;
            Connection = new SqliteConnection(_inMemoryOptions.ConnectionString);
        }

        public async Task ConnectAsync()
        {
            await Connection.OpenAsync();
        }

        public DirectoryInfo WorkingDirectory => _inMemoryOptions.DataExtensionDirectory;

        public async Task<DataTable> LookupAsync(LookupRequest lookup)
        {
            SqliteCommand sqliteLookup = SqliteSelectBuilder.FromRequest(this.Connection, lookup);

            return await GetDataTableFromCommand(QuotedIdentifier.Create(lookup.DataExtension), sqliteLookup);
        }

        public async Task InsertAsync(InsertRequest insert)
        {
            SqliteCommand sqliteLookup = SqliteInsertBuilder.FromRequest(this.Connection, insert);

            await sqliteLookup.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> GetSchemaAsync(string dataExtension)
        {
            var tableName = QuotedIdentifier.Create(dataExtension);
            SqliteCommand command = Connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";

            return await GetDataTableFromCommand(tableName, command);
        }

        /// <summary>
        /// Creates a table in SQLite
        /// </summary>
        public async Task CreateTable(string dataExtension, IEnumerable<string> columns)
        {
            SqliteCommand command = SqliteCreateTableBuilder.FromColumns(Connection, dataExtension, columns);
            _loadedTables.Add(QuotedIdentifier.Create(dataExtension));

            await command.ExecuteNonQueryAsync();
        }

        private async Task<DataTable> GetDataTableFromCommand(QuotedIdentifier table, SqliteCommand command)
        {
            await LoadIfNecessary(table);

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            var returnTable = new DataTable();
            returnTable.Load(reader);

            return returnTable;
        }

        /// <summary>
        /// Loads a CSV into memory if the requested table has not been previously loaded.
        /// </summary>
        private async Task LoadIfNecessary(QuotedIdentifier table)
        {
            if (_loadedTables.Contains(table))
            {
                return;
            }

            _loadedTables.Add(table);

            DataTable schema = await GetSchemaAsync(table.ToString());

            if (schema.Rows.Count == 0)
            {
                await this.LoadCsv(table.Unquote());
            }
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
