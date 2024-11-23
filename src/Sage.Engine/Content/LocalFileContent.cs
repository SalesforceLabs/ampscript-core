// Copyright (c) 2024, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Represents a content that's local on disk
    /// </summary>
    public class LocalFileContent : IContent
    {
        public string Name { get; private set; }

        /// <summary>
        /// Full path to the file
        /// </summary>
        public string Location { get; private set; }

        public int ContentVersion { get; private set; }

        public ContentType ContentType { get; }

        private string _fullPath;

        /// <summary>
        /// Reads from a local file on disk.
        /// </summary>
        public LocalFileContent(string fullPath, int contentVersion)
        {
            _fullPath = fullPath;
            Name = Path.GetFileNameWithoutExtension(fullPath);
            Location = fullPath;
            ContentVersion = contentVersion;
            ContentType = ContentExtensions.InferContentTypeFromFilename(fullPath);
        }

        /// <summary>
        /// Reads from a local file on disk - with an alternative name representing the content.
        /// </summary>
        public LocalFileContent(string name, string fullPath, int contentVersion, ContentType contentType)
        {
            _fullPath = fullPath;
            Name = name;
            Location = _fullPath;
            ContentVersion = contentVersion;
            ContentType = contentType;
        }

        /// <summary>
        /// Opens the file for read
        /// </summary>
        public TextReader? GetTextReader()
        {
            return new StreamReader(File.OpenRead(_fullPath));
        }
    }
}
