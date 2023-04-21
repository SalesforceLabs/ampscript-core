// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.PackageManager.DataObjects
{
    /// <summary>
    /// An asset may be a multi-channel asset and may be rendred differently based on the channel.
    ///
    /// This is accomplished through conveying a "view" of the asset.
    /// </summary>
    /// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/views.html"/>
    internal record Views(SubjectLineView? subjectline,
        PreheaderView? preheaderview,
        HtmlView? html,
        TextView? text,
        ViewAsAWebpageView? viewAsAWebPage,
        SubscriptionCenterView? subscriptioncenter,
        ForwardHtmlView? forwardHTML,
        ForwardTextView? forwardText
        )
    {
    }

    internal record SubjectLineView(string contentType, string content);
    internal record PreheaderView();
    internal record HtmlView(string content, Dictionary<string, Slot> slots);
    internal record TextView();
    internal record ViewAsAWebpageView();
    internal record SubscriptionCenterView();
    internal record ForwardHtmlView();
    internal record ForwardTextView();
}
