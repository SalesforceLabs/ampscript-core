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
        public string Compile()
        {
            return ContentCompiler.Compile(asset.data);
        }

        /// <summary>
        /// From package manager, they specify references as {{mcpm#SOME_IDENTIFIER}} (mustache expressions).  This method just updates the ID to be what integer identifier is local.
        /// </summary>
        /// <remarks>
        /// It's silly to bring a mustache expression library for one use case like this, so it just does regex matching.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FixReferences(string input)
        {
            return MatchAssetId.Replace(input, "${ID}");
        }
    }
}
