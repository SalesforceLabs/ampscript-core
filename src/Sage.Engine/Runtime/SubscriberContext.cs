// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// The subscriber context is the set of attributes unique to the individual subscriber that this
    /// message is being personalized for.  They are things such as "First name", "last name", etc.
    /// </summary>
    public class SubscriberContext
    {
        private readonly JsonNode? _context;

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
            _context = JsonNode.Parse(context, options);
        }

        public object? GetAttribute(string attributeName)
        {
            return _context?[attributeName]?.ToString();
        }
    }
}
