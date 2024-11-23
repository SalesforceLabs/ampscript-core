// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// A class that compiles AMPscript source code into C# and optionally will execute it.
    /// </summary>
    internal class AmpscriptCompiler : IAmpscriptCompiler
    {
        public bool CanCompile(IContent content)
        {
            return content.ContentType == ContentType.AMPscript;
        }

        public string Compile(CompilationOptions options, RuntimeContext runtimeContext, SubscriberContext context)
        {
            return CSharpCompiler.CompileAndExecute(options, runtimeContext, out var _);
        }
    }
}
