// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Runtime.CompilerServices;
using Sage.Engine.Runtime;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown due to a runtime error caused by the authored content.
///
/// All runtime exceptions are issues with the authored content.
/// </summary>
public class RuntimeException : Exception
{
    private RuntimeContext _context;

    /// <summary>
    /// This is the AMPSCRIPT function that threw the exception
    /// </summary>
    public string Function { get; }

    /// <summary>
    /// This is the string message of the failure
    /// </summary>
    public string RuntimeMessage { get; }

    /// <summary>
    /// This is the order of stack frames when the exception was thrown, from oldest to newest element.
    /// </summary>
    public List<StackFrame> AmpscriptCallstack { get; }

    public RuntimeException(
        string message,
        RuntimeContext context,
        [CallerMemberName] string caller = "") : base(FormatExceptionMessageWithFunction(message, context, caller))
    {
        _context = context;
        RuntimeMessage = message;
        Function = caller;

        // A copy is taken because the line number for the stack frame will change
        // after it's popped after the exception begins unwinding the stack.
        AmpscriptCallstack = context.GetStackFrames().Select(f => (StackFrame)f.Clone()).ToList();
    }

    protected static string FormatExceptionMessageWithArgumentAndFunction(
        string message,
        RuntimeContext context,
        string callerFunction,
        string callerArgument)
    {
        return $"Function: {callerFunction}\r\nArgument: {callerArgument}\r\n{message}";
    }

    protected static string FormatExceptionMessageWithFunction(
        string message,
        RuntimeContext context,
        string function)
    {
        return $"Function -- {function}\r\n\r\n{message}";
    }
}
