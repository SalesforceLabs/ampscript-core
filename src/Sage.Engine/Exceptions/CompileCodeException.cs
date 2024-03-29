﻿// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;
using Sage.Engine.Compiler;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown by a failure to compile the C# code
/// </summary>
public class CompileCodeException : GenerateCodeException
{
    public CompileResult CompileResult { get; }

    internal CompileCodeException(CompileResult compileResult) : base("Failed to build the content")
    {
        this.CompileResult = compileResult;
    }

    public override string ToString()
    {
        return string.Join("\n", CompileResult.EmitResult.Diagnostics);
    }
}
