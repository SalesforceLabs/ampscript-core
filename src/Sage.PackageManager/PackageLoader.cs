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
    /// <summary>
    /// Abstract class that can load a package and generate a package graph.
    /// This is extended by the JSON and ZIP loaders to source entities either from other files (zip), or within the paylaod
    /// (json).
    /// </summary>
    public abstract class PackageLoader : IPackageLoader
    {
        /// <summary>
        /// Loads the package manager given package into a package graph
        /// </summary>
        public virtual PackageGraph Load()
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

                foreach (JsonNode? entity in referenceArray)
                {
                    Reference? reference = entity.Deserialize<Reference>();
                    if (reference == null)
                    {
                        continue;
                    }

                    GetOrCreate(entities, referenceObject.Key).AddOutgoing(GetOrCreate(entities, reference.identifier));
                }
            }

            var selectedEntities = new HashSet<Entity>();
            SelectedEntities? selectedEntitiesObject = GetSelectedEntities().Deserialize<SelectedEntities>();
            if (selectedEntitiesObject != null)
            {
                foreach (int assetId in selectedEntitiesObject.assets)
                {
                    selectedEntities.Add(GetOrCreate(entities, $"assets/{assetId}"));
                }
            }

            return new PackageGraph(entities, selectedEntities);
        }

        /// <summary>
        /// Validates that the provided package is well formed before parsing.
        /// </summary>
        protected abstract void Validate();

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
                    PackagedAsset? packagedAsset = entityJson.Deserialize<PackagedAsset>();

                    return packagedAsset == null ? new Entity(id) : new Asset(packagedAsset);
                case "categories":
                    PackagedCategory? packagedCategory = entityJson.Deserialize<PackagedCategory>();
                    return packagedCategory == null ? new Entity(id) : new Category(packagedCategory);
                default:
                    PackagedEntity? packagedEntity = entityJson.Deserialize<PackagedEntity>();
                    return packagedEntity == null ? new Entity(id) : new Entity(packagedEntity);
            }
        }
    }
}
