// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests
{
    using System.Data;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentValidatorTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void AssertStringThrowsCorrectInformation(string? value)
        {
            RuntimeArgumentException exception = Assert.Throws<RuntimeArgumentException>(() => ArgumentValidator.ThrowIfStringNullOrEmpty(value))!;
            Assert.That(exception.ArgumentName == nameof(value));
            Assert.That(exception.CallingFunction == nameof(AssertStringThrowsCorrectInformation));
        }

        [Test]
        public void AssertDataRowAndDataType()
        {
            int intVal = 0;
            var dataTable = new DataTable();

            RuntimeArgumentException exception = Assert.Throws<RuntimeArgumentException>(() => ArgumentValidator.ThrowIfNotDataTable(intVal))!;
            Assert.That(exception.ArgumentName == nameof(intVal));
            Assert.That(exception.CallingFunction == nameof(AssertDataRowAndDataType));

            exception = Assert.Throws<RuntimeArgumentException>(() => ArgumentValidator.ThrowIfNotDataRow(dataTable))!;
            Assert.That(exception.ArgumentName == nameof(dataTable));
            Assert.That(exception.CallingFunction == nameof(AssertDataRowAndDataType));
        }
    }
}
