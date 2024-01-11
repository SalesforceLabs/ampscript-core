// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Web;

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Used to hook into the link tracking system.
        /// </summary>
        /// <remarks>There is no current support to wrap links, so this just returns the raw link</remarks>
        public string REDIRECTTO(object? url)
        {
            return url?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Modifies a string to only include characters that are safe to use in URLs.
        /// </summary>
        /// <param name="encodeAllStrings">The string to convert to a format that is safe to include in URLs.</param>
        /// <param name="encodeAllCharacters">
        /// If true, the function converts all spaces and non-ASCII characters in a URL parameter string to their hexadecimal character codes.
        ///
        /// If false, the function only converts spaces in a URL parameter string to the hexadecimal character code %20, and leaves other characters unchanged.
        ///
        /// The default value is false.</param>
        /// <param name="encodeAllStrings">
        /// If true, the function converts any text string that you pass as the first parameter into a version that is safe to use in URLs.
        ///
        /// If false, the function only converts a string into a URL-safe version if the unsafe characters are part of a URL parameter string.
        ///
        /// The default value is false.</param>
        public string URLENCODE(object urlToEncode, object? encodeAllCharacters = null, object? encodeAllStrings = null)
        {
            bool boolEncodeAllCharacters = false;

            string urlString = urlToEncode?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(urlString))
            {
                return urlString;
            }

            if (encodeAllCharacters != null)
            {
                boolEncodeAllCharacters = SageValue.ToBoolean(encodeAllCharacters);
            }

            bool boolEncodeAllStrings = false;
            if (encodeAllStrings != null)
            {
                boolEncodeAllStrings = SageValue.ToBoolean(encodeAllStrings);
            }

            string renderedUrlResults = CompileAndExecuteEmbeddedCode("urlencode", urlString) ?? string.Empty;

            string prefixPart = string.Empty;
            string partToEncode = string.Empty;

            if (boolEncodeAllStrings)
            {
                // No prefix, encoding all strings
                partToEncode = renderedUrlResults;
            }
            else
            {
                int queryStringStart = renderedUrlResults.IndexOf('?');

                if (queryStringStart > 0)
                {
                    prefixPart = renderedUrlResults.Substring(0, queryStringStart + 1);
                    partToEncode = renderedUrlResults.Substring(queryStringStart + 1);
                }
                else
                {
                    prefixPart = renderedUrlResults;
                    // No part to encode, no query string
                }
            }

            string encodedString = null;

            if (boolEncodeAllCharacters)
            {
                encodedString = HttpUtility.UrlEncode(partToEncode);
            }
            else
            {
                encodedString = partToEncode.Replace(" ", "%20");
            }

            return prefixPart + encodedString;
        }
    }
}
