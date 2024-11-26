// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// The subscriber context is the set of attributes unique to the individual subscriber that this
    /// message is being personalized for.  They are things such as "First name", "last name", etc.
    /// </summary>
    public class SubscriberContext
    {
        // TODO: This should be updated to only parse once
        private readonly JsonNode? _contextNode;
        private readonly JsonDocument? _contextDocument;

        public SubscriberContext(string? context)
        {
            if (context == null)
            {
                return;
            }

            var options = new JsonNodeOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            _contextNode = JsonNode.Parse(context, options);
            _contextDocument = JsonDocument.Parse(context);
        }

        public object? GetAttribute(string attributeName)
        {
            var result = _contextNode?[attributeName];

            if (result == null)
            {
                return $"%%{attributeName}%%";
            }

            return result;
        }

        /// <summary>
        /// Returns a flat, non-nested set of subscriber attributes as a key/value pair of strings.
        /// </summary>
        public Dictionary<string, string> GetAttributes()
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(_contextNode) ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Enables "rich" attributes, which can be complete JSON documents
        /// </summary>
        public JsonDocument? GetRichAttributes()
        {
            return _contextDocument;
        }
    }
}
