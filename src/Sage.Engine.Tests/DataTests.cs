// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.Compiler;
using Sage.Engine.Data;
using Sage.Engine.Data.DependencyInjection;
using Sage.Engine.DependencyInjection;
using Sage.Engine.Extensions;
using Sage.Engine.Runtime;
using Sage.Engine.Tests.Compatibility;

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Validates tests regarding ampscript data functions
    /// </summary>
    public class DataTests : SageTest
    {
        [Test]
        [RuntimeTest("Data")]
        public EngineTestResult TestDataFunctions(CorpusData test)
        {
            var result = TestUtils.GetOutputFromTest(_serviceProvider, test);
            Assert.That(result.Output, Is.EqualTo(test.Output));
            return result;
        }
    }

    public class DataExtensionClientTests
    {
        /// <summary>
        /// Whatever the request, returns a table with a single column and single value
        /// </summary>
        class MockDataExtensionClient : IDataExtensionClient
        {
            public void Dispose()
            {
            }

            private DataTable CreateTestTable()
            {
                DataTable table = new DataTable();
                table.Columns.Add("TestRow", typeof(string));
                DataRow row = table.NewRow();
                row["TestRow"] = "TestValue";
                table.Rows.Add(row);
                return table;
            }

            public Task<DataTable> LookupAsync(LookupRequest lookup)
            {
                return Task.FromResult(CreateTestTable());
            }

            public Task InsertAsync(InsertRequest insert)
            {
                return Task.FromResult(CreateTestTable());
            }

            public Task<DataTable> GetSchemaAsync(string dataExtension)
            {
                return Task.FromResult(CreateTestTable());
            }

            public Task ConnectAsync()
            {
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Demonstrates how a simple DE client can be created and tested
        /// </summary>
        [Test]
        public void CanOverrideDataExtensionClient()
        {
            var collection = new ServiceCollection();
            collection.AddSage();
            collection.AddScoped<IDataExtensionClient, MockDataExtensionClient>();
            ServiceProvider provider = collection.BuildServiceProvider();


            CompilationOptions options = new CompilerOptionsBuilder()
                .WithContent(new EmbeddedContent(
                    "%%=Lookup('Test','TestRow','TestRow', 'TestValue')=%%",
                    "TEST",
                    "TEST", 
                    1, 
                    ContentType.AMPscript))
                .Build();

            string result = provider.GetService<Renderer>()!.Render(options, null);

            Assert.That(result, Is.EqualTo("TestValue"));
        }
    }
}
