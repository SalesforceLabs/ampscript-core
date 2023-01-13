// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Data;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Misc;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// Holds helper functions for validating input arguments
    /// </summary>
    public static class ArgumentValidator
    {
        /// <summary>
        /// Invokes ToString on the object, throws <see cref="RuntimeArgumentException"/> if the result was null or empty
        /// </summary>
        public static string ThrowIfStringNullOrEmpty(
            [NotNull] object? input,
            [CallerMemberName] string callerFunction = "",
            [CallerArgumentExpression("input")] string callerArgument = "")
        {
            const string StringIsNullOrEmpty = "The provided string is null or empty";

            if (input == null)
            {
                throw new RuntimeArgumentException(StringIsNullOrEmpty, callerFunction, callerArgument);
            }

            string? result = input.ToString();
            if (string.IsNullOrEmpty(result))
            {
                throw new RuntimeArgumentException(StringIsNullOrEmpty, callerFunction, callerArgument);
            }

            return result;
        }

        /// <summary>
        /// Casts the object to a <see cref="DataTable"/>, throws if it is not that type
        /// </summary>
        public static DataTable ThrowIfNotDataTable(
            [NotNull] object? input,
            [CallerMemberName] string callerFunction = "",
            [CallerArgumentExpression("input")] string callerArgument = "")
        {
            return ThrowIfNotType<DataTable>(input, "ROWSET", callerFunction, callerArgument);
        }

        /// <summary>
        /// Casts the object to a <see cref="DataRow"/>, throws if it is not that type
        /// </summary>
        public static DataRow ThrowIfNotDataRow(
            [NotNull] object? input,
            [CallerMemberName] string callerFunction = "",
            [CallerArgumentExpression("input")] string callerArgument = "")
        {
            return ThrowIfNotType<DataRow>(input, "FIELD", callerFunction, callerArgument);
        }

        /// <summary>
        /// Casts the object to a <see cref="T"/>, throws if it is not that type
        /// </summary>
        public static T ThrowIfNotType<T>(
            [NotNull] object? input,
            string typeName,
            [CallerMemberName] string callerFunction = "",
            [CallerArgumentExpression("input")] string callerArgument = "") where T : class
        {
            if (input is not T convertedType || convertedType == null)
            {
                throw new RuntimeArgumentException($"The provided value is not a {typeName}", callerFunction, callerArgument);
            }

            return convertedType;
        }
    }
}
