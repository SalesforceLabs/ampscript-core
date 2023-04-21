// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using Sage.PackageManager.DataObjects;

namespace Sage.PackageManager
{
    abstract class PackageLoader
    {
        /// <summary>
        /// Validates that the provided package is well formed before parsing.
        /// </summary>
        internal abstract void Validate();

        /// <summary>
        /// Returns the JSON object
        /// </summary>
        protected abstract JsonObject? GetEntity(string id, string entityType, string entityName);

        /// <summary>
        /// Returns the SelectedEntities JSON object
        /// </summary>
        protected abstract JsonObject GetSelectedEntities();

        /// <summary>
        /// Returns the References JSON object
        /// </summary>
        protected abstract JsonObject GetReferences();

        /// <summary>
        /// Loads the package manager given package into a package graph
        /// </summary>
        internal PackageGraph Load()
        {
            Validate();

            var entities = new ConcurrentDictionary<string, Entity>();

            JsonObject referencesJsonObject = GetReferences();
            foreach (KeyValuePair<string, JsonNode?> referenceObject in referencesJsonObject)
            {
                if (referenceObject.Value is not JsonArray referenceArray)
                {
                    continue;
                }

                foreach (var entity in referenceArray)
                {
                    DataObjects.Reference? reference = entity.Deserialize<DataObjects.Reference>();
                    if (reference == null)
                    {
                        continue;
                    }

                    GetOrCreate(entities, referenceObject.Key).AddOutgoing(GetOrCreate(entities, reference.identifier));
                }
            }

            var selectedEntities = new HashSet<Entity>();
            SelectedEntities? selectedEntitiesObject = GetSelectedEntities().Deserialize<DataObjects.SelectedEntities>();
            if (selectedEntitiesObject != null)
            {
                foreach (int assetId in selectedEntitiesObject.assets)
                {
                    selectedEntities.Add(GetOrCreate(entities, $"assets/{assetId}"));
                }
            }

            return new PackageGraph(entities, selectedEntities);
        }
        internal Entity GetOrCreate(ConcurrentDictionary<string, Entity> entities, string id)
        {
            return entities.GetOrAdd(id, LoadEntity);
        }

        private Entity LoadEntity(string id)
        {
            string[] split = id.Split("/");
            string entityType = split[0];
            string entityName = split[1];

            JsonObject? entityJson = GetEntity(id, entityType, entityName);

            if (entityJson == null)
            {
                return new Entity(id);
            }

            switch (entityType)
            {
                case "assets":
                    DataObjects.PackagedAsset? packagedAsset = entityJson.Deserialize<DataObjects.PackagedAsset>();
                    return packagedAsset == null ? new Entity(id) : new Asset(packagedAsset);
                case "categories":
                    DataObjects.PackagedCategory? packagedCategory = entityJson.Deserialize<DataObjects.PackagedCategory>();
                    return packagedCategory == null ? new Entity(id) : new Category(packagedCategory);
                default:
                    DataObjects.PackagedEntity? packagedEntity = entityJson.Deserialize<DataObjects.PackagedEntity>();
                    return packagedEntity == null ? new Entity(id) : new Entity(packagedEntity);
            }
        }
    }
}
