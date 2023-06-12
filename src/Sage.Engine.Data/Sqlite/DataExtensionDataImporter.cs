// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.VisualBasic.FileIO; // TextFieldParser

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Methods which enable loading data from a CSV into a SQLite table.
    /// </summary>
    /// <remarks>
    /// While this is mostly generic for any IDataExtensionClient - creating the DE is not yet implemented at the interface level,
    /// and the fields are required for creating the table, so it cannot be pre-created.
    /// </remarks>
    internal static class DataExtensionDataImporter
    {
        /// <summary>
        /// Takes a CSV stream and loads it into a SQLite table. The first line of the stream must include headers.
        /// </summary>
        /// <remarks>
        /// This is not very efficient, do not expect great performance from large CSV files.
        /// </remarks>
        /// <param name="dataExtensionClient">The client used for loading data</param>
        /// <param name="reader">The stream of CSV data at the beginning of the stream. The first line must include header information.</param>
        /// <param name="dataExtensionName">The name of the data extension to load the data into.</param>
        /// <exception cref="InvalidDataException">If the CSV is malformed in any way</exception>
        public static async Task LoadCsv(this SqliteDataExtensionClient dataExtensionClient, Stream reader, string dataExtensionName)
        {
            var parser = new TextFieldParser(reader);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            string[] headers;
            if (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();
                if (fields == null)
                {
                    throw new InvalidDataException("No header data found");
                }

                headers = fields.ToArray();
            }
            else
            {
                return;
            }

            await dataExtensionClient.CreateTable(dataExtensionName, headers);

            var builder = new InsertRequestBuilder(dataExtensionName);
            while (!parser.EndOfData)
            {
                string[]? dataArray = parser.ReadFields();

                if (dataArray == null)
                {
                    continue;
                }

                if (headers.Length != dataArray.Length)
                {
                    throw new InvalidDataException($"Line: {parser.LineNumber} -- Not enough fields. Expected:{headers.Length} Actual:{dataArray.Length}");
                }

                for (int i = 0; i < headers.Length; i++)
                {
                    builder.WithAttribute(headers[i], dataArray[i]);
                }

                await dataExtensionClient.InsertAsync(builder.Build());
                builder = new InsertRequestBuilder(dataExtensionName);
            }
        }

        /// <summary>
        /// Load data from a CSV file into a SQLite table.  The first line of the CSV file must include headers.
        /// </summary>
        /// <remarks>
        /// The CSV must be the same name as the data extension
        /// The CSV must be in a directory named "DataExtensions".
        /// The "DataExtensions" directory must be a sub directory of <see cref="workingDirectory"/>
        ///
        /// Note: This is not very efficient, do not expect great performance from large CSV files.
        /// </remarks>
        /// <param name="dataExtensionClient">The client used for loading data</param>
        /// <param name="workingDirectory">The root folder for where the CSV files live</param>
        /// <param name="dataExtensionName">The name of the data extension to load the data into.</param>
        public static async Task LoadCsv(this SqliteDataExtensionClient dataExtensionClient, string dataExtensionName)
        {
            var csvFile = new FileInfo(Path.Join(dataExtensionClient.WorkingDirectory.FullName, $"{dataExtensionName}.csv"));

            await dataExtensionClient.LoadCsv(csvFile.OpenRead(), dataExtensionName);
        }
    }
}
