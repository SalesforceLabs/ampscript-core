// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;
using System.Text;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// Information about the currently executing stack frame of AMPscript.
    /// </summary>
    /// <remarks>The AMPscript stack is unique, in that there is no variable scoping changes, but just an output scoping</remarks>
    /// <remarks>This is also used for any compilation errors</remarks>
    [DebuggerDisplay("{OutputStream}", Name = "{Name}")]
    internal class StackFrame
    {
        public StringBuilder OutputStream
        {
            get;
        }

        public string Name
        {
            get;
        }

        public StackFrame(string name)
        {
            OutputStream = new StringBuilder();
            Name = name;
        }
    }
}
