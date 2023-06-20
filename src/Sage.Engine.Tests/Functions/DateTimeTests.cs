// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    internal class DateTimeTests : FunctionTestBase
    {
        [Test]
        [TestCase("2023-05-01 5:00:00", "5/1/2023 5:00:00 AM")]
        public void TestDateTimeParse(string input, string expected)
        {
            string actual = this._runtimeContext.V(this._runtimeContext.DATEPARSE(input));
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
