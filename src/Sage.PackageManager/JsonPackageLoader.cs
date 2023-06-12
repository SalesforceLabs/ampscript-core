// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace Sage.PackageManager
{
    /// <summary>
    /// Loads a PackageManager package from an exported JSON file.
    /// </summary>
    public class JsonPackageLoader : PackageLoader
    {
        private JsonNode? _jsonNode;
        private readonly JsonPackageOptions _options;

        public JsonPackageLoader(
            IOptions<JsonPackageOptions> options)
        {
            _options = options.Value;
        }

        public override PackageGraph Load()
        {
            string jsonContents = File.ReadAllText(_options.Source.FullName);
            _jsonNode = JsonNode.Parse(jsonContents)!;

            if (_jsonNode == null)
            {
                throw new InvalidDataException("Package is empty");
            }

            return base.Load();
        }

        protected override JsonObject GetSelectedEntities()
        {
            Debug.Assert(_jsonNode != null);
            return _jsonNode["selectedEntities"] as JsonObject ?? throw new InvalidDataException("No SelectedEntities found in package");
        }

        protected override JsonObject GetReferences()
        {
            Debug.Assert(_jsonNode != null);
            return _jsonNode["references"] as JsonObject ?? throw new InvalidDataException("No references found in package");
        }

        protected override void Validate()
        {
            // TODO Ensure it has selected entities and references
        }

        protected override JsonObject? GetEntity(string id, string entityType, string entityName)
        {
            Debug.Assert(_jsonNode != null);
            return _jsonNode["entities"]?[entityType]?[entityName] as JsonObject;
        }
    }
}
