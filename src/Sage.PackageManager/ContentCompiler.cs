// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using HtmlAgilityPack;
using Sage.PackageManager.DataObjects;

namespace Sage.PackageManager
{
    /// <summary>
    /// The content builder asset model creates a mechanism that refers to slots and content blocks via data-type and data-key values in the content.
    ///
    /// This class is responsible for taking a package graph and flattening the content.
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/compiling-asset-model.html"/>
    internal static class ContentCompiler
    {
        /// <summary>
        /// Identifies all data-key divs and recursively resolves the underlying referenced content.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private static HtmlNode FlattenPlaceholder(Placeholder placeholder, int stackDepth)
        {
            // There is some documentation about how deep this can go:
            // https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/slots.html
            // This value is much more generous than the documentation - but will still prevent any stack overflows.
            if (stackDepth > 20)
            {
                throw new StackOverflowException("Slot content is too deep");
            }

            var flattenedPlaceholder = new HtmlDocument();
            flattenedPlaceholder.OptionOutputOriginalCase = true;
            flattenedPlaceholder.OptionCheckSyntax = false;
            flattenedPlaceholder.GlobalAttributeValueQuote = AttributeValueQuote.Initial;
            flattenedPlaceholder.LoadHtml(placeholder.content);

            HtmlNodeCollection dataKeyNodes = flattenedPlaceholder.DocumentNode.SelectNodes("//*[@data-key]");

            if (dataKeyNodes == null)
            {
                return flattenedPlaceholder.DocumentNode;
            }

            foreach (HtmlNode? childPlaceholderNode in dataKeyNodes)
            {
                string? dataType = childPlaceholderNode?.GetAttributeValue("data-type", null);
                string? dataKey = childPlaceholderNode?.GetAttributeValue("data-key", null);

                if (childPlaceholderNode == null || dataKey == null || dataType == null)
                {
                    throw new InvalidDataException($"Input malformed, cannot find data-type attribute for node '{dataKey}' at {childPlaceholderNode?.Line}:{childPlaceholderNode?.LinePosition}");
                }

                Placeholder? childPlaceholder;
                switch (dataType)
                {
                    case "block":
                        childPlaceholder = placeholder.blocks?[dataKey];
                        break;
                    case "slot":
                        childPlaceholder = placeholder.slots?[dataKey];
                        break;
                    default:
                        throw new InvalidDataException($"Unsupported data type {dataType} for node {dataKey}");
                }

                if (childPlaceholder == null)
                {
                    throw new InvalidDataException($"Unable to find placeholder for {dataKey}");
                }

                HtmlNode flattenedChildPlaceholder = FlattenPlaceholder(childPlaceholder, stackDepth + 1);

                childPlaceholderNode.ParentNode.ReplaceChild(flattenedChildPlaceholder, childPlaceholderNode);
            }

            return flattenedPlaceholder.DocumentNode;
        }

        /// <summary>
        /// Compiles the HTMLView of the underlying asset through identifying data-key divs and recursively
        /// resolving the underlying referenced content.
        /// </summary>
        internal static string Compile(DataObjects.Asset asset)
        {
            HtmlView? htmlView = asset.views?.html;

            if (htmlView == null)
            {
                throw new InvalidDataException("No html content found");
            }

            if (string.IsNullOrEmpty(htmlView.content))
            {
                return string.Empty;
            }

            var htmlPlaceholder = new Placeholder(htmlView.content, null, htmlView.slots);
            HtmlNode flattenedPlaceholder = FlattenPlaceholder(htmlPlaceholder, 0);
            return flattenedPlaceholder.InnerHtml ?? string.Empty;
        }
    }
}