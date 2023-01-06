// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class SageValueTests
    {
        [Test]
        [TestCase(1, 1, true)]
        [TestCase("1", 1, false)]
        [TestCase("-1", -1, false)]
        [TestCase(null, 0, false)]
        [TestCase(1.0, 1, true)]
        [TestCase(1.1, 1, false)]
        [TestCase("true", 0, false)]
        public void TestToLong(object? data, long expected, bool expectedAbleToConvert)
        {
            bool couldConvert = SageValue.TryToLong(data, out long result);

            Assert.That(couldConvert, Is.EqualTo(expectedAbleToConvert), $"Conversion result not what was expected.  {data} of {data?.GetType()} expected {expectedAbleToConvert}, got {couldConvert}");
            Assert.That(result, Is.EqualTo(expected), "Converted result is not what was expected");

            if (expectedAbleToConvert == false)
            {
                Assert.Throws<InvalidCastException>(() => { SageValue.ToLong(data!); });
            }
            else
            {
                Assert.That(expected, Is.EqualTo(SageValue.ToLong(data!)));
            }
        }
    }
}
