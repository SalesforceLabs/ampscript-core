// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;

// Ignore the following in this file - mostly due to enabling this codebase to adhere to these rules but AMPscript code may not.
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConstantConditionalAccessQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Formats the output
        /// </summary>
        public string FORMAT(object subject, object formatStringObject, object? type = null, object? culture = null)
        {
            string? formatString = formatStringObject.ToString();
            if (formatString == null)
            {
                return subject.ToString() ?? string.Empty;
            }

            string? compareType = type?.ToString()?.Trim();

            CultureInfo? cultureInfo = null;
            if (culture != null)
            {
                string? cultureString = culture.ToString();
                if (cultureString != null)
                {
                    cultureInfo = new CultureInfo(cultureString);
                }
            }

            cultureInfo ??= this._currentCulture;

            // !Backward compatibility!
            // If converting fails, return the source.
            try
            {
                if (string.Compare("date", compareType, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return SageValue.ToString(SageValue.ToDateTime(subject.ToString(), this._currentCulture, DateTimeStyles.AssumeLocal), formatString, cultureInfo);
                }

                if (string.Compare("number", compareType, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return SageValue.ToString(SageValue.ToDecimal(subject), formatString, cultureInfo);
                }

                return SageValue.ToString(subject, formatString, cultureInfo);
            }
            catch (Exception)
            {
                return subject.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Similar to a ternary operator - evaluates <see cref="expression"/> and if it evaluates to true, returns <see cref="leftResult"/>.  Otherwise, returns <see cref="rightResult"/>
        /// </summary>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="leftResult">The result to be returned if the expression evaluates to true</param>
        /// <param name="rightResult">The result to be returned if the expression evaluates to false</param>
        /// <returns>Either <see cref="leftResult"/> or <see cref="rightResult"/>, depending on the result of <see cref="expression"/></returns>
        public object IIF(object expression, object leftResult, object rightResult)
        {
            bool expressionResult = SageValue.ToBoolean(expression);

            if (expressionResult)
            {
                return leftResult;
            }

            return rightResult;
        }

        /// <summary>
        /// Returns a true value if the specified parameter is null.
        /// </summary>
        public bool ISNULL(object expression)
        {
            object? unboxed = SageValue.UnboxVariable(expression);

            return unboxed is DBNull;
        }

        /// <summary>
        /// Outputs to where this block appears in the content.
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public void OUTPUT(object data)
        {
            Output(data);
        }

        /// <summary>
        /// Outputs to where this block appears in the content, with a newline
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public void OUTPUTLINE(object data)
        {
            OutputLine(data);
        }

        /// <summary>
        /// Outputs a variable where this appears in the content
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public string V(object? data)
        {
            return SageValue.ToString(data, this._currentCulture);
        }

        /// <summary>
        /// Provides the value of the attribute
        /// </summary>
        /// <param name="attributeName">Name of the attribute to obtain</param>
        /// <returns>The value of the attribute</returns>

        public object? ATTRIBUTEVALUE(object? attributeName)
        {
            string attributeNameString = this.ThrowIfStringNullOrEmpty(attributeName);

            return GetSubscriberContext().GetAttribute(attributeNameString);
        }
    }
}
