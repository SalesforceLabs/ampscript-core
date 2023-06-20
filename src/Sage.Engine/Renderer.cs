// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Compiler;
using Sage.Engine.Runtime;

namespace Sage.Engine
{
    /// <summary>
    /// A class that compiles AMPscript source code into C#
    /// </summary>
    public class Renderer
    {
        private readonly IServiceProvider _services;

        public Renderer(
            IServiceProvider services)
        {
            _services = services;
        }

        public string Render(
            CompilationOptions compOptions,
            SubscriberContext? context = null)
        {
            var runtimeContext = new RuntimeContext(_services, compOptions, context);

            return CSharpCompiler.CompileAndExecute(compOptions, runtimeContext, out var _);
        }
    }
}
