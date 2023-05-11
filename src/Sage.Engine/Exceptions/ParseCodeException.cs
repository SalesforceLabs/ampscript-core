// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;
using System.CodeDom.Compiler;
using Sage.Engine.Compiler;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown by a failure to compile the C# code
/// </summary>
public class ParseCodeException : GenerateCodeException
{
    public RuleContext ParseContext { get; }


    internal ParseCodeException(string message, RuleContext context) : base(message)
    {
        ParseContext = context;
    }

    public override string ToString()
    {
        return $"{Message}\r\nLine(s): {ParseContext.SourceInterval}\r\nText: {ParseContext.GetText()}";
    }
}
