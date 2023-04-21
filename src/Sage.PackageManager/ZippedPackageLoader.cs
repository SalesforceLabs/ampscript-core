// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.Json.Nodes;

namespace Sage.PackageManager
{
    /// <summary>
    /// Loads a PackageManager package from an unzipped package manager zip file
    /// </summary>
    internal class ZippedPackageLoader : PackageLoader
    {
        private readonly string _packagePath;

        private string ReferencesFile => Path.Combine(_packagePath, "references.json");
        private string InfoFile => Path.Combine(_packagePath, "info.json");
        private string SelectedEntitiesFile => Path.Combine(_packagePath, "selected-entities.json");
        private string AssetsDirectory => Path.Combine(_packagePath, "entities", "assets");
        private string CategoriesDirectory => Path.Combine(_packagePath, "entities", "categories");

        internal ZippedPackageLoader(DirectoryInfo zipPath)
        {
            if (!zipPath.Exists)
            {
                throw new ArgumentException(zipPath.FullName, $"{zipPath.FullName} is not a directory");
            }

            _packagePath = zipPath.FullName;
        }

        protected override JsonObject GetSelectedEntities()
        {
            return JsonNode.Parse(File.ReadAllText(SelectedEntitiesFile)) as JsonObject
                   ?? throw new InvalidDataException("No SelectedEntities found in package");
        }

        protected override JsonObject GetReferences()
        {
            return JsonNode.Parse(File.ReadAllText(ReferencesFile)) as JsonObject
                   ?? throw new InvalidDataException("No References found in package");
        }

        internal override void Validate()
        {
            if (!File.Exists(ReferencesFile))
            {
                throw new FileNotFoundException(ReferencesFile);
            }
            if (!File.Exists(InfoFile))
            {
                throw new FileNotFoundException(InfoFile);
            }
            if (!Directory.Exists(AssetsDirectory))
            {
                throw new DirectoryNotFoundException(AssetsDirectory);
            }
            if (!Directory.Exists(CategoriesDirectory))
            {
                throw new FileNotFoundException(CategoriesDirectory);
            }
        }

        protected override JsonObject? GetEntity(string id, string entityType, string entityName)
        {
            string pathToFile = Path.Combine(_packagePath, "entities", entityType, $"{entityName}.json");
            if (!File.Exists(pathToFile))
            {
                return null;
            }

            return JsonNode.Parse(File.ReadAllText(pathToFile)) as JsonObject;
        }
    }
}
