// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Sage.Engine.Compiler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

namespace Sage.Engine
{
    /// <summary>
    /// A class that compiles AMPscript source code into C#
    /// </summary>
    public static class Renderer
    {
        public static string Render(string file)
        {
            var options = new CompilationOptions(file, "", OptimizationLevel.Debug);
            return CSharpCompiler.CompileAndExecute(options);
        }
    }
}
