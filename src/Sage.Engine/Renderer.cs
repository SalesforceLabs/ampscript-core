// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Compiler;
using Sage.Engine.Runtime;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

namespace Sage.Engine
{
    /// <summary>
    /// A class that compiles AMPscript source code into C#
    /// </summary>
    public static class Renderer
    {
        /// <summary>
        /// Renders the content with the provided subscriber context.
        ///
        /// If no subscriber context is provided, it is read from a `subscriber.json` file.
        /// </summary>
        /// <param name="file">The file to render</param>
        /// <param name="context">The subscriber context - if none is provided, it may be automatically read generated from a `subscriber.json` file.</param>
        /// <returns></returns>
        public static string Render(FileInfo file, SubscriberContext? context = null)
        {
            CompilationOptions options = new CompilerOptionsBuilder()
                .WithInputFile(file)
                .Build();

            RuntimeContext runtimeContext = new RuntimeContext(options, context);

            return CSharpCompiler.CompileAndExecute(options, runtimeContext);
        }
    }
}
