// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using HandlebarsDotNet;
using HandlebarsDotNet.Extension.Json;
using Sage.Engine.Compiler;
using Sage.Engine.Runtime;

namespace Sage.Engine.Handlebars
{
    /// <summary>
    /// Uses HandlebarsDotNet to compile content to a string.
    ///
    /// Does not support AMPscript embedded in the content.
    /// </summary>
    internal class HandlebarsCompiler : IHandlebarsCompiler
    {
        private IHandlebars _handlebar;

        public HandlebarsCompiler()
        {
            _handlebar = HandlebarsDotNet.Handlebars.Create();
            _handlebar.Configuration.UseJson();
        }

        public bool CanCompile(IContent content)
        {
            return content.ContentType == ContentType.Handlebars;
        }

        public string Compile(CompilationOptions options, RuntimeContext runtimeContext, SubscriberContext? subscriberContext)
        {
            var template = _handlebar.Compile(options.Content.GetTextReader());

            StringWriter writer = new StringWriter();

            template(writer, subscriberContext?.GetRichAttributes());

            return writer.ToString();
        }
    }
}
