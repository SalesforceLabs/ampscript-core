// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// This class holds helpers to convert boxed values to other types.
    /// </summary>
    public static class SageValue
    {
        #region BOXING

        /// <summary>
        /// Returns whether or not the provided inputObj can be converted to a number
        /// </summary>
        /// <param name="inputObj"></param>
        /// <returns></returns>
        public static bool IsNumberLike(object? inputObj)
        {
            if (inputObj is null)
            {
                return false;
            }

            if (inputObj is int or long or decimal or double)
            {
                return true;
            }

            if (inputObj is string valueString)
            {
                return decimal.TryParse(valueString, out _);
            }

            return false;
        }

        /// <summary>
        /// Attempts to convert the boxed inputObj to a long.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not long is the best representation of this boxed inputObj</returns>
        public static bool TryToLong(object? inputObj, out long result)
        {
            if (inputObj == null)
            {
                result = default;
                return false;
            }

            if (inputObj is int intObj)
            {
                result = intObj;
                return true;
            }

            if (inputObj is long longObj)
            {
                result = longObj;
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

            try
            {
                // Return false but still convert, because strings can convert - it's just not the optimal representation
                result = Convert.ToInt64(inputObj);
                return false;
            }
            catch
            {
                result = 0;
            }

            return false;
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to a long and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to a long</param>
        /// <returns>The long representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to a long inputObj.</exception>
        public static long ToLong(object inputObj)
        {
            if (!TryToLong(inputObj, out long result))
            {
                throw new InvalidCastException($"Cannot convert {inputObj} to long");
            }

            return result;
        }

        /// <summary>
        /// Attempts to convert the boxed inputObj to a double.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not long is the best representation of this boxed inputObj</returns>
        public static bool TryToDouble(object? inputObj, out double result)
        {
            if (inputObj == null)
            {
                result = 0;
                return false;
            }

            if (inputObj is double doubleVal)
            {
                result = doubleVal;
                return true;
            }

            if (inputObj is long or decimal or int)
            {
                result = Convert.ToDouble(inputObj);
                return true;
            }

            if (inputObj is string stringVal)
            {
                return double.TryParse(stringVal, out result);
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to a double and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to a double</param>
        /// <returns>The double representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to a double inputObj.</exception>
        public static double ToDouble(object inputObj)
        {
            if (!TryToDouble(inputObj, out double result))
            {
                throw new InvalidCastException($"Cannot convert {inputObj} to double");
            }

            return result;
        }

        /// <summary>
        /// Attempts to convert the boxed inputObj to a decimal.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not decimal is the best representation of this boxed inputObj</returns>
        public static bool TryToDecimal(object? inputObj, out decimal result)
        {
            if (inputObj is decimal or double)
            {
                result = (decimal)inputObj;
                return true;
            }

            // Return false but still convert, because strings can convert just not the optimal representation
            result = Convert.ToDecimal(inputObj);
            return false;
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to a decimal and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to a decimal</param>
        /// <returns>The decimal representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to a decimal inputObj.</exception>
        public static decimal ToDecimal(object? inputObj)
        {
            if (!TryToDecimal(inputObj, out decimal result))
            {
                throw new InvalidCastException($"Cannot convert {inputObj} to decimal");
            }

            return result;
        }

        /// <summary>
        /// Attempts to convert the boxed inputObj to an int.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not int is the best representation of this boxed inputObj</returns>
        public static bool TryToInt(object inputObj, out int result)
        {
            if (inputObj is int objInt)
            {
                result = objInt;
                return true;
            }

            if (TryToLong(inputObj, out long longResult))
            {
                result = (int)longResult;
                if (longResult < int.MinValue || longResult > int.MaxValue)
                {
                    return false;
                }

                return true;
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to an int and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to an int</param>
        /// <returns>The int representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to an int inputObj.</exception>
        public static int ToInt(object inputObj)
        {
            if (!TryToInt(inputObj, out int intResult))
            {
                throw new InvalidCastException($"{inputObj} can not be converted to an integer");
            }

            return intResult;
        }


        /// <summary>
        /// Attempts to convert the boxed inputObj to a DateTimeOffset.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not DateTimeOffset is the best representation of this boxed inputObj</returns>
        public static bool TryToDateTime(object? inputObj, out DateTimeOffset result)
        {
            if (inputObj is DateTimeOffset dateTimeObj)
            {
                result = dateTimeObj;
                return true;
            }

            DateTimeOffset.TryParse(inputObj?.ToString(), out result);

            return false;
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to DateTimeOffset and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to an DateTimeOffset</param>
        /// <returns>The DateTimeOffset representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to an DateTimeOffset inputObj.</exception>
        public static DateTimeOffset ToDateTime(object? inputObj)
        {
            if (!TryToDateTime(inputObj, out DateTimeOffset result))
            {
                throw new InvalidCastException($"Cannot convert {inputObj} to DateTimeOffset");
            }

            return result;
        }

        /// <summary>
        /// Attempts to convert the boxed inputObj to a bool.
        /// </summary>
        /// <param name="inputObj">The input boxed inputObj</param>
        /// <param name="result">The result of the conversion</param>
        /// <returns>Whether or not bool is the best representation of this boxed inputObj</returns>
        public static bool TryToBoolean(object inputObj, out bool result)
        {
            if (inputObj is bool boolObj)
            {
                result = boolObj;
                return true;
            }

            if (TryToInt(inputObj, out int intVal))
            {
                result = intVal == 1;
                return false;
            }

            return bool.TryParse(inputObj.ToString(), out result);
        }

        /// <summary>
        /// Assumes the boxed inputObj can be converted to bool and converts it.
        /// </summary>
        /// <param name="inputObj">Boxed inputObj to convert to an bool</param>
        /// <returns>The bool representation of the object</returns>
        /// <exception cref="InvalidCastException">When the inputObj cannot be unboxed to an bool inputObj.</exception>
        public static bool ToBoolean(object inputObj)
        {
            if (!TryToBoolean(inputObj, out bool result))
            {
                throw new InvalidCastException($"Cannot convert {inputObj} to bool");
            }

            return result;
        }
        #endregion

        #region COMPARE
        // .net 5 changed from NLS to ICU to be more compatible with Linux.
        // The Marketing Cloud ampscript looks to follow NLS, but this workaround appears to fix it:
        // https://github.com/dotnet/runtime/issues/20599
        private static readonly StringComparer s_stringComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace);

        /// <summary>
        /// A set of comparision operators supported in the language
        /// </summary>
        public enum CompareOperator
        {
            LessThan,
            LessThanOrEqual,
            EqualTo,
            NotEqualTo,
            GreaterThan,
            GreaterThanOrEqual
        };

        /// <summary>
        /// Converts the result of a comparison (-1, 0, 1) to a boolean representation (== / !=)
        /// </summary>
        /// <param name="op">The type of operation performed</param>
        /// <param name="result">The result of that operation</param>
        /// <returns>The boolean representation of the comparision</returns>
        private static bool ConvertCompareToBool(CompareOperator op, int result)
        {
            switch (op)
            {
                case CompareOperator.LessThan:
                    return result < 0;
                case CompareOperator.LessThanOrEqual:
                    return result <= 0;
                case CompareOperator.EqualTo:
                    return result == 0;
                case CompareOperator.NotEqualTo:
                    return result != 0;
                case CompareOperator.GreaterThanOrEqual:
                    return result >= 0;
                case CompareOperator.GreaterThan:
                    return result > 0;
            }

            throw new InternalEngineException("Invalid comparision");
        }

        /// <summary>
        /// Whether or not the provided comparision operator checks for equality
        /// </summary>
        /// <param name="op">The operation requested</param>
        /// <returns>If the operator checks for equality</returns>
        public static bool IsEqualToken(this CompareOperator op)
        {
            return op == CompareOperator.EqualTo ||
                   op == CompareOperator.GreaterThanOrEqual ||
                   op == CompareOperator.LessThanOrEqual;
        }

        public static bool LessThan(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.LessThan);
        }
        public static bool LessThanOrEqual(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.LessThanOrEqual);
        }
        public static bool GreaterThan(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.GreaterThan);
        }
        public static bool GreaterThanOrEqual(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.GreaterThanOrEqual);
        }
        public static bool EqualTo(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.EqualTo);
        }
        public static bool NotEqualTo(object? left, object? right)
        {
            return CompareTo(left, right, CompareOperator.NotEqualTo);
        }
        public static bool CompareTo(object? left, object? right, CompareOperator op)
        {
            if (TryCompareNulls(left, right, out int nullResult))
            {
                if (op.IsEqualToken())
                {
                    return nullResult == 0;
                }

                return nullResult != 0;
            }

            if (left == null || right == null)
            {
                throw new InternalEngineException($"Comparision continuing after null checks with provided null value. Left: {left} Right: {right}");
            }

            if (TryCompareBooleans(left, right, out int boolResult))
            {
                if (boolResult == 0)
                {
                    return IsEqualToken(op);
                }

                return !IsEqualToken(op);
            }

            if (TryCompareDateTime(left, right, out int dateTimeResult))
            {
                return ConvertCompareToBool(op, dateTimeResult);
            }

            if (TryCompareNumbers(left, right, out int numberResult))
            {
                return ConvertCompareToBool(op, numberResult);
            }

            if (TryCompareStrings(left, right, op, out int stringResult))
            {
                return ConvertCompareToBool(op, stringResult);
            }

            throw new InternalEngineException("Invalid comparision");
        }

        private static bool TryCompareNulls(object? left, object? right, out int result)
        {
            if (left != null && right != null)
            {
                result = -1;
                return false;
            }

            if (left == null && right == null)
            {
                result = 0;
            }
            else
            {
                result = -1;
            }

            return true;
        }

        /// <summary>
        /// Compares two DateTimes
        /// </summary>
        private static bool TryCompareDateTime(object left, object right, out int result)
        {
            // NOTE! By-design this is not a lazily-checked or statement.  We want values for both left and right, and if this were ||,
            // then the right side may not be evaluated!
            if (TryToDateTime(left, out DateTimeOffset leftDateTime) |
                TryToDateTime(right, out DateTimeOffset rightDateTime))
            {
                result = leftDateTime.CompareTo(rightDateTime);
                return true;
            }

            result = -1;
            return false;
        }

        /// <summary>
        /// Compares two booleans
        /// </summary>
        private static bool TryCompareBooleans(object left, object right, out int result)
        {
            // NOTE! By-design this is not a lazily-checked or statement.  We want values for both left and right, and if this were ||,
            // then the right side may not be evaluated!
            if (TryToBoolean(left, out bool leftBool) |
                TryToBoolean(right, out bool rightBool))
            {
                result = leftBool.CompareTo(rightBool);
                return true;
            }

            result = -1;
            return false;
        }

        /// <summary>
        /// Compares two numbers, if either one is a number.
        ///
        /// Uses decimal comparision first, then goes to a long comparision.
        /// </summary>
        private static bool TryCompareNumbers(object? left, object? right, out int result)
        {
            if (IsNumberLike(left) == false && IsNumberLike(right) == false)
            {
                result = -1;
                return false;
            }

            // NOTE! By-design this is not a lazily-checked or statement.  We want values for both left and right, and if this were ||,
            // then the right side may not be evaluated!
            if (TryToLong(left, out long leftLong) | 
                TryToLong(right, out long rightLong))
            {
                result = leftLong.CompareTo(rightLong);
                return true;
            }

            // NOTE! By-design this is not a lazily-checked or statement.  We want values for both left and right, and if this were ||,
            // then the right side may not be evaluated!
            if (TryToDecimal(left, out decimal leftDecimal) | 
                TryToDecimal(right, out decimal rightDecimal))
            {
                result = leftDecimal.CompareTo(rightDecimal);
                return true;
            }

            result = -1;
            return false;
        }

        /// <summary>
        /// Compares two strings. As this is the last resort - anything converts to a string.
        ///
        /// String comparisons are case-insensitive in the language.
        /// </summary>
        private static bool TryCompareStrings(object left, object right, CompareOperator op, out int result)
        {
            string leftResult = left.ToString()?.ToLower() ?? string.Empty;
            string rightResult = right.ToString()?.ToLower() ?? string.Empty;

            switch (op)
            {
                case CompareOperator.LessThan:
                case CompareOperator.LessThanOrEqual:
                case CompareOperator.GreaterThanOrEqual:
                case CompareOperator.GreaterThan:
                    result = s_stringComparer.Compare(leftResult, rightResult);
                    return true;
                case CompareOperator.EqualTo:
                case CompareOperator.NotEqualTo:
                    result = string.Equals(leftResult, rightResult, StringComparison.OrdinalIgnoreCase) ? 0 : -1;
                    return true;
            }

            throw new InternalEngineException("Strings came back as not strings! What gives?");
        }
        #endregion
    }
}
