// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Runtime.CompilerServices;

// Ignore the following in this file - mostly due to enabling this codebase to adhere to these rules but AMPscript code may not.
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConstantConditionalAccessQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
#pragma warning disable IDE0060 // Remove unused parameter

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Impression regions allow for tracking specific regions of content.
        /// </summary>
        /// <param name="regionName">The name of the impression region to track</param>
        /// <remarks>At this time, no tracking features are available so this function is a no-op. It exists solely for AMPscript to compile.</remarks>
        public string BEGINIMPRESSIONREGION(object? regionName)
        {
            return "";
        }

        /// <summary>
        /// Impression regions allow for tracking specific regions of content.
        /// </summary>
        /// <param name="regionName">The name of the impression region to track</param>
        /// <remarks>At this time, no tracking features are available so this function is a no-op.  It exists solely for AMPscript to compile.</remarks>
        public string ENDIMPRESSIONREGION()
        {
            return "";
        }

        /// <summary>
        /// This function serves as a marker for link attribution within the content.
        /// </summary>
        /// <param name="assetBlock">The identifier of the asset block</param>
        /// <remarks>At this time, no tracking features are available so this function is a no-op. It exists solely for AMPscript to compile.</remarks>
        public string BEGINASSETBLOCK(object? assetBlock)
        {
            return "";
        }

        /// <summary>
        /// This function serves as a marker for link attribution within the content.
        /// </summary>
        /// <param name="assetBlock">The identifier of the asset block</param>
        /// <remarks>At this time, no tracking features are available so this function is a no-op. It exists solely for AMPscript to compile.</remarks>
        public string ENDASSETBLOCK()
        {
            return "";
        }

        /// <summary>
        /// Returns content stored in Classic Content by ID.
        /// </summary>
        /// <remarks>To use content builder content blocks, see <see cref="CONTENTBLOCKBYID"/>, <see cref="CONTENTBLOCKBYKEY"/> or <see cref="CONTENTBLOCKBYNAME"/>.</remarks>
        /// <param name="contentAreaId">The ID of the content area.  This can be found in the properties dialog of the content area folder.</param>
        /// <param name="impressionRegionName">The name of the impression region to associated with this content</param>
        /// <param name="throwIfNotFound">If the content could not be found, should a runtime error be thrown?</param>
        /// <param name="defaultContent">If there is no content found, what should be returned?</param>
        /// <param name="success">Status code of the result. Pass in a parameter to this function. 0 indicates success, -1 indicates failure.</param>
        /// <returns>The results of reading and executing the content stored in the content area</returns>
        public string CONTENTAREA(object contentAreaId, object? impressionRegionName = null, object? throwIfNotFound = null, object? defaultContent = null, SageVariable? success = null)
        {
            string id = this.ThrowIfStringNullOrEmpty(contentAreaId);

            string? executionResults =
                CompileAndExecuteEmbeddedCode($"contentareaid__{id}",
                    () => GetClassicContentClient().GetContentById(this.ThrowIfStringNullOrEmpty(id)));

            return ReturnContentBasedOnInput(executionResults, id, throwIfNotFound, defaultContent, success);
        }

        /// <summary>
        /// Returns content stored in Classic Content by name.
        /// </summary>
        /// <remarks>To use content builder content blocks, see <see cref="CONTENTBLOCKBYID"/>, <see cref="CONTENTBLOCKBYKEY"/> or <see cref="CONTENTBLOCKBYNAME"/>.</remarks>
        /// <param name="contentAreaName">The name of the content area.</param>
        /// <param name="impressionRegionName">The name of the impression region to associated with this content</param>
        /// <param name="throwIfNotFound">If the content could not be found, should a runtime error be thrown?</param>
        /// <param name="defaultContent">If there is no content found, what should be returned?</param>
        /// <param name="success">Status code of the result. Pass in a parameter to this function. 0 indicates success, -1 indicates failure.</param>
        /// <returns>The results of reading and executing the content stored in the content area</returns>
        public string CONTENTAREABYNAME(object contentAreaName, object? impressionRegionName = null, object? throwIfNotFound = null, object? defaultContent = null, SageVariable? success = null)
        {
            string id = this.ThrowIfStringNullOrEmpty(contentAreaName);

            string? executionResults =
                CompileAndExecuteEmbeddedCode($"contentareaname__{id}",
                    () => GetClassicContentClient().GetContentByName(this.ThrowIfStringNullOrEmpty(id)));

            return ReturnContentBasedOnInput(executionResults, id, throwIfNotFound, defaultContent, success);
        }

        /// <summary>
        /// Returns content stored in Content Builder by name.
        /// </summary>
        /// <remarks>To use classic content areas, see <see cref="CONTENTAREA"/> or <see cref="CONTENTAREABYNAME"/>.</remarks>
        /// <param name="contentBlockName">The name of the content block.</param>
        /// <param name="impressionRegionName">The name of the impression region to associated with this content</param>
        /// <param name="throwIfNotFound">If the content could not be found, should a runtime error be thrown?</param>
        /// <param name="defaultContent">If there is no content found, what should be returned?</param>
        /// <param name="success">Status code of the result. Pass in a parameter to this function. 0 indicates success, -1 indicates failure.</param>
        /// <returns>The results of reading and executing the content stored in the content area</returns>
        public string CONTENTBLOCKBYNAME(object contentBlockName, object? impressionRegionName = null, object? throwIfNotFound = null, object? defaultContent = null, SageVariable? success = null)
        {
            string id = this.ThrowIfStringNullOrEmpty(contentBlockName);

            string? executionResults =
                CompileAndExecuteEmbeddedCode($"contentblockname__{id}",
                    () => GetContentBuilderContentClient().GetContentByName(this.ThrowIfStringNullOrEmpty(id)));

            return ReturnContentBasedOnInput(executionResults, id, throwIfNotFound, defaultContent, success);
        }

        /// <summary>
        /// Returns content stored in Content Builder by ID.
        /// </summary>
        /// <remarks>To use classic content areas, see <see cref="CONTENTAREA"/> or <see cref="CONTENTAREABYNAME"/>.</remarks>
        /// <param name="contentBlockId">The ID of the content block.  This can be found in the properties menu in content builder.</param>
        /// <param name="impressionRegionName">The name of the impression region to associated with this content</param>
        /// <param name="throwIfNotFound">If the content could not be found, should a runtime error be thrown?</param>
        /// <param name="defaultContent">If there is no content found, what should be returned?</param>
        /// <param name="success">Status code of the result. Pass in a parameter to this function. 0 indicates success, -1 indicates failure.</param>
        /// <returns>The results of reading and executing the content stored in the content area</returns>
        public string CONTENTBLOCKBYID(object contentBlockId, object? impressionRegionName = null, object? throwIfNotFound = null, object? defaultContent = null, SageVariable? success = null)
        {
            string id = this.ThrowIfStringNullOrEmpty(contentBlockId);

            string? executionResults =
                CompileAndExecuteEmbeddedCode($"contentblockid__{id}",
                    () => GetContentBuilderContentClient().GetContentById(this.ThrowIfStringNullOrEmpty(id)));

            return ReturnContentBasedOnInput(executionResults, id, throwIfNotFound, defaultContent, success);
        }

        /// <summary>
        /// Returns content stored in Content Builder by the customer key specified.
        /// </summary>
        /// <remarks>To use classic content areas, see <see cref="CONTENTAREA"/> or <see cref="CONTENTAREABYNAME"/>.</remarks>
        /// <param name="contentBlockKey">The customer key of the content block.  This can be found in the properties menu in content builder.</param>
        /// <param name="impressionRegionName">The name of the impression region to associated with this content</param>
        /// <param name="throwIfNotFound">If the content could not be found, should a runtime error be thrown?</param>
        /// <param name="defaultContent">If there is no content found, what should be returned?</param>
        /// <param name="success">Status code of the result. Pass in a parameter to this function. 0 indicates success, -1 indicates failure.</param>
        /// <returns>The results of reading and executing the content stored in the content area</returns>
        public string CONTENTBLOCKBYKEY(object contentBlockKey, object? impressionRegionName = null, object? throwIfNotFound = null, object? defaultContent = null, SageVariable? success = null)
        {
            string id = this.ThrowIfStringNullOrEmpty(contentBlockKey);

            string? executionResults =
                CompileAndExecuteEmbeddedCode($"contentblockkey__{id}",
                    () => GetContentBuilderContentClient().GetContentByCustomerKey(this.ThrowIfStringNullOrEmpty(id)));

            return ReturnContentBasedOnInput(executionResults, id, throwIfNotFound, defaultContent, success);
        }

        /// <summary>
        /// Evalutes the string as if it were code.  Similar to JavaScript `eval()`.
        /// </summary>
        /// <param name="content">The content to be parsed</param>
        /// <returns>The rendered text</returns>
        public string TREATASCONTENT(object? content)
        {
            string contentString = content?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(contentString))
            {
                return contentString;
            }

            return CompileAndExecuteEmbeddedCode($"treatascontent", contentString) ?? string.Empty;
        }

        /// <summary>
        /// Identical to TREATASCONTENT, except the results are cached and made available to all compute nodes for performance optimization.
        /// </summary>
        /// <remarks>
        /// The primary use of TREATASCONTENTAREA vs TREATASCONTENT is for performance improvement for content that is relatively static, expensive to generate, and frequently used.
        /// </remarks>
        public string TREATASCONTENTAREA(object? key, object? content, object? impressionRegionName = null)
        {
            return TREATASCONTENT(content);
        }

        /// <summary>
        /// All of the content block functions validate the results the same way, so this helper method does this
        /// </summary>
        private string ReturnContentBasedOnInput(
            string? result,
            string? id,
            object? throwIfNotFound,
            object? defaultContent,
            SageVariable? success,
            [CallerMemberName] string caller = "")
        {
            bool throwIfNotFoundBool = true;
            if (throwIfNotFound != null)
            {
                throwIfNotFoundBool = SageValue.ToBoolean(throwIfNotFound);
            }

            string defaultContentString = string.Empty;
            if (defaultContent != null)
            {
                defaultContentString = this.ThrowIfStringNullOrEmpty(defaultContent, caller);
            }

            if (result == null)
            {
                if (success != null)
                {
                    success.Value = -1;
                }

                if (throwIfNotFoundBool)
                {
                    throw new RuntimeException(
                        $"Content block '{id}' does not exist and 'throwIfNotFound' was evaluated as true",
                        this,
                        caller);
                }

                return defaultContentString;
            }

            if (success != null)
            {
                success.Value = 0;
            }

            return result;
        }
    }
}
