// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager
{
    /// <summary>
    /// A representation of a packaged asset in PackageManager.
    /// </summary>
    public class Asset : Entity
    {
        private readonly DataObjects.PackagedAsset asset;

        internal Asset(DataObjects.PackagedAsset asset) : base(asset)
        {
            this.asset = asset;
        }

        /// <summary>
        /// Compiles the given asset and returns the rendered.
        /// </summary>
        /// <remarks>
        /// This does not do any ampscript rendering.  "Compile" and "rendered" in this context simply means
        /// resolving the data-key references and placing them into the content tree.
        /// </remarks>
        public string Compile(PackageGraph referenceGraph)
        {
            return ContentCompiler.Compile(referenceGraph, asset.data);
        }
    }
}
