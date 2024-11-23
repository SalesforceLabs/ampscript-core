// Copyright (c) 2024, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Compiler;

namespace Sage.Engine.Content
{
    public interface IContentClient
    {
        /// <summary>
        /// Obtains a piece of content by a given name
        /// </summary>
        /// <param name="name">Name of the content area to return, including the path within the my contents folder if necessary</param>
        /// <returns>A local cached file on disk that contains the content</returns>
        IContent? GetContentByName(string name);

        /// <summary>
        /// Obtains a piece of content by a given ID
        /// </summary>
        /// <param name="name">The ID of the specified content</param>
        /// <returns>A local cached file on disk that contains the content</returns>
        IContent? GetContentById(string id);

        /// <summary>
        /// Obtains a piece of content by a given customer key
        /// </summary>
        /// <param name="name">The customer key of the specified content</param>
        /// <returns>A local cached file on disk that contains the content</returns>
        IContent? GetContentByCustomerKey(string customerKey);
    }

    public interface IClassicContentClient : IContentClient
    {

    }

    public interface IContentBuilderContentClient : IContentClient
    {

    }
}
