// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// The package graph is the representation of a downloaded package that is easily consumable by users of this library.
    ///
    /// It contains all entities, relationships to each other, along with mechanisms to obtain the metadata.
    /// </summary>
    public class PackageGraph
    {
        private readonly IReadOnlyDictionary<string, Entity> _entities;
        public readonly IReadOnlySet<Entity> SelectedEntities;

        /// <summary>
        /// Creates a package graph derived from a zip file that was downloaded from the package manager.
        /// </summary>
        public static PackageGraph FromZip(DirectoryInfo zipPath)
        {
            var graph = new ZippedPackageLoader(zipPath);

            return graph.Load();
        }
        /// <summary>
        /// Creates a package graph derived from a zip file that was downloaded from the package manager.
        /// </summary>
        public static PackageGraph FromJson(FileInfo jsonPath)
        {
            var graph = new JsonPackageLoader(File.ReadAllText(jsonPath.FullName));

            return graph.Load();
        }

        /// <summary>
        /// Returns an asset from the given ID
        /// </summary>
        public Asset? GetAsset(int id)
        {
            _entities.TryGetValue($"assets/{id}", out Entity? result);
            return result as Asset;
        }

        internal PackageGraph(IReadOnlyDictionary<string, Entity> entities, IReadOnlySet<Entity> selectedEntities)
        {
            _entities = entities;
            SelectedEntities = selectedEntities;
        }
    }
}