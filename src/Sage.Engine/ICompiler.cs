// Copyright (c) 2024, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Compiler;
using Sage.Engine.Runtime;

namespace Sage.Engine
{
    /// <summary>
    /// Renders a piece of content to a string.
    /// </summary>
    public interface ICompiler
    {
        bool CanCompile(IContent type);

        string Compile(CompilationOptions options, RuntimeContext runtimeContext, SubscriberContext context);
    }
}
