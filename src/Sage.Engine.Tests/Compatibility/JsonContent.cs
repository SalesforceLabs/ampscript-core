// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace Sage.Engine.Tests.Compatibility
{
    /// <summary>
    /// Helper to add the media type for the requests
    /// </summary>
    public class JsonContent : StringContent
    {
        public JsonContent(JsonObject content) : base(content.ToString())
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        public JsonContent(string content) : base(content)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}
