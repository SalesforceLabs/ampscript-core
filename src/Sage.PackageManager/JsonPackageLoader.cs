// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.Json.Nodes;

namespace Sage.PackageManager
{
    /// <summary>
    /// Loads a PackageManager package from an exported JSON file.
    /// </summary>
    internal class JsonPackageLoader : PackageLoader
    {
        private readonly JsonNode _jsonNode;

        internal JsonPackageLoader(string jsonContents)
        {
            _jsonNode = JsonNode.Parse(jsonContents)!;

            if (_jsonNode == null)
            {
                throw new InvalidDataException("Package is empty");
            }
        }

        protected override JsonObject GetSelectedEntities()
        {
            return _jsonNode["selectedEntities"] as JsonObject ?? throw new InvalidDataException("No SelectedEntities found in package");
        }

        protected override JsonObject GetReferences()
        {
            return _jsonNode["references"] as JsonObject ?? throw new InvalidDataException("No references found in package");
        }

        internal override void Validate()
        {
            // TODO Ensure it has selected entities and references
        }

        /// <inheritdoc cref="PackageLoader.GetEntity"/>
        protected override JsonObject? GetEntity(string id, string entityType, string entityName)
        {
            return _jsonNode["entities"]?[entityType]?[entityName] as JsonObject;
        }
    }
}
