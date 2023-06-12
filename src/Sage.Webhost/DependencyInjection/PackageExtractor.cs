// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.Options;
using Sage.PackageManager;

namespace Sage.Webhost.DependencyInjection
{
    /// <summary>
    /// PackageManager packages need to be processed for a few things:
    /// * Their content is stored in the JSON payload and needs extracted to disk to be able to set breakpoints
    /// * This JSON payload needs flattened, resolving data-key references
    /// * Specific entity references need resolved - aka changing ContentBlockByKey('{{mcpm#/some/path/to/some/file}}') to
    /// ContentBlockByKey('bar')
    /// This class is responsible for doing so
    /// </summary>
    public class PackageExtractor
    {
        private readonly PackageExtractorOptions _options;
        private readonly IPackageLoader _packageLoader;

        public PackageExtractor(
            IPackageLoader packageLoader,
            IOptions<PackageExtractorOptions> jsonPackageExtractorOptions)
        {
            _packageLoader = packageLoader;
            _options = jsonPackageExtractorOptions.Value;
        }

        public void Extract()
        {
            PackageGraph graph = _packageLoader.Load();

            Asset? asset = graph.GetAsset(_options.AssetId);

            if (asset == null)
            {
                throw new InvalidDataException("Asset not found");
            }

            string compiledContent = asset.Compile(graph);
            File.WriteAllText(_options.GeneratedFilename.FullName, compiledContent);

            foreach (Entity entity in graph.Entities)
            {
                if (entity is Asset entityAsset)
                {
                    string entityOutputPath =
                        Path.Combine(_options.ContentDirectory.FullName, $"{entityAsset.Id}.generated.ampscript");
                    File.WriteAllText(entityOutputPath, entityAsset.Compile(graph));
                }
            }
        }
    }
}
