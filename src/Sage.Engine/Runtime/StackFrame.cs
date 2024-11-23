﻿// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;
using System.Text;
using Sage.Engine.Compiler;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// Information about the currently executing stack frame of AMPscript.
    /// </summary>
    /// <remarks>The AMPscript stack is unique, in that there is no variable scoping changes, but just an output scoping</remarks>
    /// <remarks>This is also used for any compilation errors</remarks>
    [DebuggerDisplay("{OutputStream}", Name = "{Name}")]
    public class StackFrame : ICloneable
    {
        public StringBuilder OutputStream
        {
            get;
        }

        /// <summary>
        /// The name of the new context
        /// </summary>
        /// <example>GETCONTENTBLOCKBYNAME("Foo") => Name == "Foo"</example>
        /// <example>GETCONTENTBLOCKBYID("Foo") => Name == "Foo"</example>
        public string Name
        {
            get;
        }

        public IContent Content
        {
            get;
        }

        /// <summary>
        /// For code that came from TREATASCONTENT that does not have a file
        /// </summary>
        public string? GeneratedCode
        {
            get;
        }

        /// <summary>
        /// The line number from the calling context
        /// </summary>
        public int CurrentLineNumber
        {
            get;
            set;
        }

        public StackFrame(string name, IContent content)
        {
            OutputStream = new StringBuilder();
            Name = name;
            Content = content;
        }

        public StackFrame(string name, string generatedCode)
        {
            OutputStream = new StringBuilder();
            Name = name;
            GeneratedCode = generatedCode;
        }

        private StackFrame(string name, int lineNumber, IContent content, string? generatedCode, StringBuilder outputStream)
        {
            Name = name;
            CurrentLineNumber = lineNumber;
            Content = content;
            GeneratedCode = generatedCode;
            OutputStream = new StringBuilder(outputStream.ToString());
        }

        public object Clone()
        {
            return new StackFrame(Name, CurrentLineNumber, Content, GeneratedCode, OutputStream);
        }
    }
}
