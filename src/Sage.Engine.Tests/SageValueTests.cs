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
        [TestCase(1, 1, SageValue.UnboxResult.Succeed)]
        [TestCase("1", 1, SageValue.UnboxResult.SucceededFromString)]
        [TestCase("-1", -1, SageValue.UnboxResult.SucceededFromString)]
        [TestCase(null, 0, SageValue.UnboxResult.Fail)]
        [TestCase(1.0, 1, SageValue.UnboxResult.Succeed)]
        [TestCase(1.1, 0, SageValue.UnboxResult.Fail)]
        [TestCase("true", 0, SageValue.UnboxResult.Fail)]
        public void TestToLong(object? data, long expected, SageValue.UnboxResult expectedAbleToConvert)
        {
            SageValue.UnboxResult couldConvert = SageValue.TryToLong(data, out long result);

            Assert.That(couldConvert, Is.EqualTo(expectedAbleToConvert), $"Conversion result not what was expected.  {data} of {data?.GetType()} expected {expectedAbleToConvert}, got {couldConvert}");
            Assert.That(result, Is.EqualTo(expected), "Converted result is not what was expected");

            if (expectedAbleToConvert == SageValue.UnboxResult.Fail)
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
