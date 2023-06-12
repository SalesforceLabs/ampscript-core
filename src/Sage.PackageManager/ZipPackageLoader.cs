// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace Sage.PackageManager
{
    /// <summary>
    /// Loads a PackageManager package from an unzipped package manager zip file
    /// </summary>
    public class ZipPackageLoader : PackageLoader
    {
        private readonly string? _packagePath;

        private readonly ZipPackageOptions _options;

        private string ReferencesFile => Path.Combine(_packagePath!, "references.json");
        private string InfoFile => Path.Combine(_packagePath!, "info.json");
        private string SelectedEntitiesFile => Path.Combine(_packagePath!, "selected-entities.json");
        private string AssetsDirectory => Path.Combine(_packagePath!, "entities", "assets");
        private string CategoriesDirectory => Path.Combine(_packagePath!, "entities", "categories");

        public ZipPackageLoader(
            IOptions<ZipPackageOptions> options)
        {
            _options = options.Value;
            _packagePath = _options.PackageDirectory.FullName;
        }

        public override PackageGraph Load()
        {
            if (!_options.PackageDirectory.Exists)
            {
                throw new ArgumentException(_options.PackageDirectory.FullName, $"{_options.PackageDirectory.FullName} is not a directory");
            }

            return base.Load();
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

        protected override void Validate()
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
            Debug.Assert(_packagePath != null);
            string pathToFile = Path.Combine(_packagePath, "entities", entityType, $"{entityName}.json");
            if (!File.Exists(pathToFile))
            {
                return null;
            }

            return JsonNode.Parse(File.ReadAllText(pathToFile)) as JsonObject;
        }
    }
}
