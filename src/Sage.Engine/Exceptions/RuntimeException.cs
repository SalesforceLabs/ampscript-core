// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Runtime.CompilerServices;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown due to a runtime error caused by the authored content.
///
/// All runtime exceptions are issues with the authored content.
/// </summary>
internal class RuntimeException : Exception
{
    public RuntimeException(
        string message,
        [CallerMemberName] string caller = "") : base(FormatExceptionMessageWithFunction(message, caller))
    {
    }

    protected static string FormatExceptionMessageWithArgumentAndFunction(
        string message,
        string callerFunction,
        string callerArgument)
    {
        return $"Function: {callerFunction}\r\nArgument: {callerArgument}\r\n{message}";
    }

    protected static string FormatExceptionMessageWithFunction(string message, string function)
    {
        return $"Function -- {function}\r\n\r\n{message}";
    }
}
