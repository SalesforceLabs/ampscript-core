// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    internal class StringTests : FunctionTestBase
    {
        [Test]
        [TestCase("Hello World", "Hello ", "World")]
        [TestCase("Hello World", "Hello ", null, "World")]
        [TestCase("Hello 1World", "Hello ", 1, "World")]
        [TestCase("1World", null, 1, "World")]
        public void TestConcat(string expected, params object?[] arguments)
        {
            string actual = this._runtimeContext.CONCAT(arguments);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Hello World", "Hello ", 1)]
        [TestCase("Hello World", "hello ", 1)]
        [TestCase("Hello", "o", 5)]
        [TestCase(null, "o", 0)]
        [TestCase(null, null, 0)]
        [TestCase("Hello", null, 0)]
        public void TestIndexOf(string? subject, string? search, int expected)
        {
            int actual = this._runtimeContext.INDEXOF(subject, search);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
