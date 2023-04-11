// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;
using StackFrame = Sage.Engine.Runtime.StackFrame;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Provides a nicer debugger view of the runtime context which only shows the content + variables.
    ///
    /// All other items in the runtime context are not important when debugging ampscript content.
    /// </summary>
    internal class RuntimeContextDebugView
    {
        public RuntimeContextDebugView(RuntimeContext original)
        {
            this.Variables = original.GetVariables().ToList();
            this.ContentStackFrames = original.GetStackFrames().ToList();
        }

        public List<StackFrame> ContentStackFrames { get; }

        public List<SageVariable> Variables { get; }
    }
}
