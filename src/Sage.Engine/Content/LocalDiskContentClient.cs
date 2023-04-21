// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Content
{
    /// <summary>
    /// Implements the IContentClient class through files on the local disk.
    /// </summary>
    public class LocalDiskContentClient : IContentClient
    {
        private readonly string _contentDirectory;

        /// <summary>
        /// Creates a content client where files of the specified name are read and returned.
        /// </summary>
        /// <param name="contentDirectory">The directory with the content files.</param>
        /// </param>
        public LocalDiskContentClient(string contentDirectory)
        {
            _contentDirectory = contentDirectory;
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public FileInfo? GetContentByName(string name)
        {
            return GetContentFromPath(name);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public FileInfo? GetContentById(string id)
        {
            return GetContentFromPath(id);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public FileInfo? GetContentByCustomerKey(string customerKey)
        {
            return GetContentFromPath(customerKey);
        }

        /// <summary>
        /// Returns content from a file with the specified name in the directory provided at the construction of this object.
        /// </summary>,
        private FileInfo? GetContentFromPath(string path)
        {
            string finalPath = Path.Combine(_contentDirectory, $"{path}.ampscript");

            if (!File.Exists(finalPath))
            {
                return null;
            }

            return new FileInfo(finalPath);
        }
    }
}