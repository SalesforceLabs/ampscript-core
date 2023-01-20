// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    [TestFixture]
    internal class UtilityTests : FunctionTestBase
    {
        [Test]
        [TestCase(null, "")]
        [TestCase("Hello", "Hello")]
        [TestCase(1, "1")]
        [TestCase(true, "True")]
        public void TestOutputAndV(object data, string expected)
        {
            this._runtimeContext.OUTPUT(data);
            Assert.That(this._runtimeContext.PopContext(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("Hello", "Hello")]
        [TestCase(1, "1")]
        [TestCase(true, "True")]
        public void TestOutputLine(object data, string expected)
        {
            this._runtimeContext.OUTPUTLINE(data);

            Assert.That(this._runtimeContext.PopContext(), Is.EqualTo(expected + Environment.NewLine));
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("Hello", "Hello")]
        [TestCase(1, "1")]
        [TestCase(true, "True")]
        public void TestV(object data, string expected)
        {
            string result = this._runtimeContext.V(data);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
