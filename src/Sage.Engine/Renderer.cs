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
        private readonly IEnumerable<ICompiler> _compilers;

        public Renderer(
            IServiceProvider services,
            IEnumerable<ICompiler> compilers)
        {
            _services = services;
            _compilers = compilers;
        }

        public string Render(
            CompilationOptions compOptions,
            SubscriberContext? context = null)
        {
            var runtimeContext = new RuntimeContext(_services, compOptions, context);

            foreach (var compiler in _compilers)
            {
                if (compiler.CanCompile(compOptions.Content))
                {
                    return compiler.Compile(compOptions, runtimeContext, context);
                }
            }

            throw new InternalEngineException("Failed to compile any content");
        }
    }
}
