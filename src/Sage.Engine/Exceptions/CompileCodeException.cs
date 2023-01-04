// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Compiler;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown by a failure to compile the C# code
/// </summary>
internal class CompileCodeException : Exception
{
    private readonly CompileResult _result;

    public CompileCodeException(CompileResult result)
    {
        this._result = result;
    }

    public override string ToString()
    {
        return string.Join("\n", _result.EmitResult.Diagnostics);
    }
}
