// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Tests.Functions
{
    internal class DataTests : FunctionTestBase
    {
        [Test]
        public void Lookup()
        {
            var emailAddress = this._runtimeContext.LOOKUP("Loyalty", "EmailAddress", "SubscriberKey", "1");
            Assert.That(emailAddress, Is.EqualTo("donnie@northerntrailoutfitters.com"));
        }

        [Test]
        public void LookupWithSpaceHeader()
        {
            var emailAddress = this._runtimeContext.LOOKUP("Loyalty", "EmailAddress", "First Name", "donnie");
            Assert.That(emailAddress, Is.EqualTo("donnie@northerntrailoutfitters.com"));
        }
    }
}
