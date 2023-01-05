// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// This class holds helpers to convert boxed values to other types.
    /// </summary>
    public static class SageValue
    {
        /// <summary>
        /// Attempts to convert the boxed value to a long.
        /// </summary>
        /// <param name="inputObj">The input boxed value</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not long is the best representation of this boxed value</returns>
        public static bool TryToLong(object? inputObj, out long result)
        {
            if (inputObj == null)
            {
                result = default;
                return false;
            }

            if (inputObj is int || inputObj is long)
            {
                result = (long)inputObj;
                return true;
            }

            if (inputObj is double doubleVal && Math.Round(doubleVal, 0) == doubleVal)
            {
                result = (long)doubleVal;
                return true;
            }
            if (inputObj is decimal decimalVal && Math.Round(decimalVal, 0) == decimalVal)
            {
                result = (long)decimalVal;
                return true;
            }

            if (inputObj is string stringValue)
            {
                return long.TryParse(stringValue, out result);
            }

            // Return false but still convert, because strings can convert - it's just not the optimal representation
            result = Convert.ToInt64(inputObj);
            return false;
        }

        /// <summary>
        /// Assumes the boxed value can be converted to a long and converts it.
        /// </summary>
        /// <param name="target">Boxed value to convert to a long</param>
        /// <returns>The long representation of the object</returns>
        /// <exception cref="InvalidCastException">When the value cannot be unboxed to a long value.</exception>
        public static long ToLong(object target)
        {
            if (!TryToLong(target, out long result))
            {
                throw new InvalidCastException($"Cannot convert {target} to long");
            }

            return result;
        }
    }
}
