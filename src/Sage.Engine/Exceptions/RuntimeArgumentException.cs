﻿// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine;

/// <summary>
/// An exception that's thrown due to a runtime error caused by the authored content.
///
/// All runtime exceptions are issues with the authored content.
/// </summary>
internal class RuntimeArgumentException : RuntimeException
{
    public string ArgumentName
    {
        get;
    }

    public string CallingFunction
    {
        get;
    }

    public RuntimeArgumentException(
        string message,
        RuntimeContext context,
        string function,
        string name) 
        : base(FormatExceptionMessageWithArgumentAndFunction(message, context, function, name), context, function)
    {
        ArgumentName = name;
        CallingFunction = function;
    }
}
