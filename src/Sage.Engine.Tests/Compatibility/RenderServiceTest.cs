// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketingCloudIntegration.Render;
using Microsoft.Extensions.DependencyInjection;

namespace Sage.Engine.Tests.Compatibility
{
    internal class RenderServiceTest : SageTest
    {

        [Test]
        [Category("Compatibility")]
        public async Task TestRender()
        {
            var result = await _serviceProvider.GetRequiredService<IRenderService>().Render("SMS", new RenderRequest()
            {
                attributes = new Dictionary<string, string>(),
                context = "Preview",
                content = "%%=ADD(1,2)=%%",
                recipient = new Recipient
                {
                    contactKey = "sample@example.com"
                }
            });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.renderedContent, Is.EqualTo("3"));
        }
    }
}
