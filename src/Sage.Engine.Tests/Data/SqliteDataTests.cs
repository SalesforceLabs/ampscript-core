// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.Data;
using Sage.Engine.Data.Sqlite;

namespace Sage.Engine.Tests.Data
{
    [TestFixture]
    internal class SqliteDataTests : SageTest
    {
        private SqliteDataExtensionClient _dataExtensionClient;

        [SetUp]
        public void TestSetup()
        {
            _dataExtensionClient = (SqliteDataExtensionClient)_serviceProvider.GetRequiredService<IDataExtensionClient>();
        }

        [TearDown]
        public void TestTearDown()
        {
            _dataExtensionClient.Dispose();
        }

        [Test]
        public async Task InitInMemory()
        {
            await _dataExtensionClient.ConnectAsync();
        }

        [Test]
        public async Task LoadDataFromTable()
        {
            await _dataExtensionClient.ConnectAsync();

            await _dataExtensionClient.CreateTable("TestTable", new[] { "TestColumn" });

            DataTable schema = await _dataExtensionClient.GetSchemaAsync("TestTable");

            Assert.That(schema.Rows.Count, Is.EqualTo(1));
            Assert.That(schema.Rows[0]["name"], Is.EqualTo("TESTCOLUMN"));
            Assert.That(schema.Rows[0]["type"], Is.EqualTo(SqliteType.Text.ToString().ToUpper()));
        }

        [Test]
        public async Task InsertAndLookup()
        {
            await _dataExtensionClient.ConnectAsync();

            await _dataExtensionClient.CreateTable("TestTable", new[] { "TestColumn" });

            var insertBuilder = new InsertRequestBuilder("TestTable");
            insertBuilder.WithAttribute("TestColumn", "Foo");
            await _dataExtensionClient.InsertAsync(insertBuilder.Build());

            var lookupBuilder = new LookupRequestBuilder("TestTable");
            lookupBuilder.WithReturnedProperty("TestColumn");
            lookupBuilder.WithConstraint("TestColumn", "Foo");
            DataTable results = await _dataExtensionClient.LookupAsync(lookupBuilder.Build());

            Assert.That(results.Rows.Count, Is.EqualTo(1));
            Assert.That(results.Rows[0]["TestColumn"], Is.EqualTo("Foo"));
        }

        [Test]
        [TestCase("LOYALTY")]
        public async Task LoadCsv(string dataExtension)
        {
            await _dataExtensionClient.ConnectAsync();
            await _dataExtensionClient.LoadCsv(dataExtension);
        }

        [Test]
        public async Task TestLoyaltyQueries()
        {
            await _dataExtensionClient.ConnectAsync();
            await _dataExtensionClient.LoadCsv("LOYALTY");

            var lookupBuilder = new LookupRequestBuilder("Loyalty");
            lookupBuilder.WithReturnedProperty("EmailAddress");
            lookupBuilder.WithConstraint("SubscriberKey", "1");
            DataTable results = await _dataExtensionClient.LookupAsync(lookupBuilder.Build());

            Assert.That(results.Rows.Count, Is.EqualTo(1));
            Assert.That(results.Rows[0]["EmailAddress"], Is.EqualTo("donnie@northerntrailoutfitters.com"));
        }
    }
}

