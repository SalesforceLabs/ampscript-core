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
        public async Task<FileInfo?> GetContentByNameAsync(string name)
        {
            return await GetContentFromPath(name);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public async Task<FileInfo?> GetContentByIdAsync(string id)
        {
            return await GetContentFromPath(id);
        }

        /// <summary>
        /// Returns content from a file with the specified name
        /// </summary>
        public async Task<FileInfo?> GetContentByCustomerKeyAsync(string customerKey)
        {
            return await GetContentFromPath(customerKey);
        }

        /// <summary>
        /// Returns content from a file with the specified name in the directory provided at the construction of this object.
        /// </summary>,
        private Task<FileInfo?> GetContentFromPath(string path)
        {
            string finalPath = Path.Combine(_contentDirectory, $"{path}.ampscript");

            if (!File.Exists(finalPath))
            {
                return Task.FromResult<FileInfo?>(null);
            }

            return Task.FromResult<FileInfo?>(new FileInfo(finalPath));
        }
    }
}