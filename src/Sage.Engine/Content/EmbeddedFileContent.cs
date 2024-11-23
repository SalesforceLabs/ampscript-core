// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Content that is contained within other content.
    /// </summary>
    /// <remarks>
    /// Recursive rendering, such as TREATASCONTENT, enables referencing content within content. This is the representation of that.
    /// </remarks>
    public class EmbeddedContent : IContent
    {
        public string Name { get; private set; }

        public string Location { get; }

        public int ContentVersion { get; private set; }

        public ContentType ContentType { get; }

        private string _content;

        public EmbeddedContent(string content, string name, string location, int contentVersion, ContentType type)
        {
            _content = content;
            Name = name;
            Location = location;
            ContentVersion = contentVersion;
            ContentType = type;
        }

        public TextReader? GetTextReader()
        {
            return new StringReader(_content);
        }
    }
}
