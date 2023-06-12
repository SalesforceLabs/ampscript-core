// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.RegularExpressions;
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
        private static readonly Regex s_referenceRegex = new Regex(@"{{mcpm#/entities/(?<entityname>.*)/(?<id>.*)/data/(?<key>.*)}}");

        /// <summary>
        /// Identifies all data-key divs and recursively resolves the underlying referenced content.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private static HtmlNode FlattenPlaceholder(PackageGraph referenceGraph, Placeholder placeholder, int stackDepth)
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

            var fixedHtmlContent = FixReferences(referenceGraph, placeholder.content);
            flattenedPlaceholder.LoadHtml(fixedHtmlContent);

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

                HtmlNode flattenedChildPlaceholder = FlattenPlaceholder(referenceGraph, childPlaceholder, stackDepth + 1);

                childPlaceholderNode.ParentNode.ReplaceChild(flattenedChildPlaceholder, childPlaceholderNode);
            }

            return flattenedPlaceholder.DocumentNode;
        }

        /// <summary>
        /// Compiles the HTMLView of the underlying asset through identifying data-key divs and recursively
        /// resolving the underlying referenced content.
        /// </summary>
        internal static string Compile(PackageGraph referenceGraph, DataObjects.Asset asset)
        {
            string? content;
            Dictionary<string, Slot>? slots;

            if (asset.assetType.id.IsBlock() || asset.assetType.id.IsTemplate())
            {
                content = asset.content;
                slots = asset.slots;
            }
            else if (asset.assetType.id.IsImage())
            {
                return asset.file ?? string.Empty;
            }
            else
            {
                content = asset?.views?.html?.content;
                slots = asset?.views?.html?.slots;
            }

            if (content == null && slots == null)
            {
                throw new InvalidDataException($"Unable to parse the content & slots for {asset?.name}");
            }

            var htmlPlaceholder = new Placeholder(content, null, slots);
            HtmlNode flattenedPlaceholder = FlattenPlaceholder(referenceGraph, htmlPlaceholder, 0);
            return flattenedPlaceholder.InnerHtml ?? string.Empty;
        }

        internal static string FixReferences(PackageGraph referenceGraph, string input)
        {
            return s_referenceRegex.Replace(input, evaluator => evaluator.Groups["id"].Value);
        }
    }
}