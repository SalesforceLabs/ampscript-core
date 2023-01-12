// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Data.Sqlite;

namespace Sage.Engine.Tests.Data
{
    [TestFixture]
    internal class QuoteTests
    {
        [Test]
        [TestCase("", "[]")]
        [TestCase("]", "[]]]")]
        [TestCase("[Hello world]", "[Hello world]")]
        [TestCase("[Hello world]]", "[[Hello world]]]]]")]
        [TestCase("[[[Hello world", "[[[[Hello world]")]
        public void Quote(string input, string expected)
        {
            Assert.That(QuotedIdentifier.Create(input).ToString(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("[Hello world]", "Hello world")]
        [TestCase("Hello world", "Hello world")]
        [TestCase("[]", "")]
        [TestCase("", "")]
        [TestCase("[]]", "[]]")]
        public void Unquote(string input, string expected)
        {
            Assert.That(QuotedIdentifier.Create(input).Unquote(), Is.EqualTo(expected));
        }
    }
}
