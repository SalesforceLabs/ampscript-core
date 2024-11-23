// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.Options;
using Sage.Engine.Compiler;

namespace Sage.Engine.Content
{
    /// <summary>
    /// Implements the IContentClient class through files on the local disk.
    /// </summary>
    public class LocalDiskContentClient : IClassicContentClient, IContentBuilderContentClient
    {
        private readonly ContentOptions _options;

        /// <summary>
        /// Creates a content client where files of the specified name are read and returned.
        /// </summary>
        /// <param name="contentDirectory">The directory with the content files.</param>
        /// </param>
        public LocalDiskContentClient(
            IOptionsSnapshot<ContentOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public IContent? GetContentByName(string name)
        {
            return GetContentFromPath(name);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public IContent? GetContentById(string id)
        {
            return GetContentFromPath(id);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public IContent? GetContentByCustomerKey(string customerKey)
        {
            return GetContentFromPath(customerKey);
        }

        /// <summary>
        /// Returns content from a file with the specified name in the directory provided at the construction of this object.
        /// </summary>,
        private IContent? GetContentFromPath(string id)
        {
            string filePath = Path.Combine(_options.InputDirectory.FullName, $"{id}.ampscript");
            if (File.Exists(filePath))
            {
                return new LocalFileContent(id, filePath, 1, ContentType.AMPscript);
            }

            return null;
        }
    }
}
