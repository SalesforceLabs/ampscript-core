// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// All variables that are set are set with this type.
    /// </summary>
    /// <remarks>
    /// Some AMPscript functions allow "output" variables that data is written to before the function returns.
    /// An example is <see cref="CONTENTBLOCKBYNAME"/>, where the "Success" parameter is a variable that can
    /// output.
    ///
    /// The original AMPscript function translation works by boxing all types into 'object', instead of some other
    /// specific type. This worked until a need for output parameters came into place.  Without modification,
    /// if that output 'object' type was a boxed 'bool' variable - there would be no way to know what the
    /// name of the parameter that the content wanted updated.  It would only be potentially possible if it
    /// were passed by ref instead of by value - but even that poses challenges for optional parameters & generating
    /// the right 'ref variable' C# syntax.
    ///
    /// The solution was to implement a new object type that boxes each _variable_.  In this way, when the
    /// parameter is passed in, it can be marked as a variable. This variable as part of the box has a name,
    /// and then the variable can be updated as part of executing this function so that when it returns, it has
    /// the correct value.
    /// </remarks>
    [DebuggerDisplay("{Value}", Name = "{Name}", Type = "Variable")]
    public class SageVariable
    {
        public string Name { get; }
        public object? Value;

        public SageVariable(string name)
        {
            this.Name = name;
        }

        public static implicit operator bool(SageVariable other)
        {
            return SageValue.EqualTo(true, other);
        }

        public override string? ToString()
        {
            return Value?.ToString();
        }
    }
}
