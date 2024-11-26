// Copyright (c) 2024, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// The type of content that is represented
    /// </summary>
    public enum ContentType
    {
        AMPscript,
        Handlebars
    };

    /// <summary>
    /// Represents content that can be compiled by this project
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// This is a human-readable name of the piece of content
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This is a human-readable location where the file can be found.
        /// It can be a filepath, URL, or any other identifier relevant to the type of content.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Supports content versioning
        /// </summary>
        public int ContentVersion { get; }

        /// <summary>
        /// Type of content this represents
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// Obtains a reader from this content to be read and compiled
        /// </summary>
        /// <returns></returns>
        public TextReader? GetTextReader();
    }
}
