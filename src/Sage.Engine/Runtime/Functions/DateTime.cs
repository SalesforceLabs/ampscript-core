// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Returns the current system (server) date and time.
        /// Now() is in Central Standard Time (CST) without daylight saving time.
        /// </summary>
        /// <param name="useSendTimeStarted">Determines whether to preserve the email sent time for post-send resolution of Now(). A value of true preserves the email sent time.</param>
        public DateTimeOffset NOW(object? useSendTimeStarted = null)
        {
            if (useSendTimeStarted != null)
            {
                throw new NotImplementedException("useSendTimeStarted is not implemented");
            }

            // TODO: Use CST without daylight savings
            return DateTimeOffset.Now;
        }

        /// <summary>
        /// Returns the difference between two dates using the specified part
        /// </summary>
        /// <param name="start">The date from which to subtract from</param>
        /// <param name="end">The date </param>
        /// <param name="diffType">Which part of the date to subtract. Valid options are "Y" (year), "M" (month), "D" (day), "H" (hour), "MI" (minute)</param>
        /// <returns></returns>
        public long DATEDIFF(object? start, object? end, object? diffType)
        {
            // **NOTE** the left side is the 'end' date, and the right side is the 'start' date.
            DateTimeOffset leftDateTime = SageValue.ToDateTime(end, null, DateTimeStyles.AssumeLocal);
            DateTimeOffset rightDateTime = SageValue.ToDateTime(start, null, DateTimeStyles.AssumeLocal);
            string? partString = diffType?.ToString()?.ToLower();

            switch (partString)
            {
                case "y":
                    return leftDateTime.Year - rightDateTime.Year;
                case "m":
                    return (leftDateTime.Month - rightDateTime.Month) + (12 * (leftDateTime.Year - rightDateTime.Year));
                case "d":
                    return (long)(new DateTime(leftDateTime.Year, leftDateTime.Month, leftDateTime.Day)
                        - new DateTime(rightDateTime.Year, rightDateTime.Month, rightDateTime.Day)).TotalDays;
                case "h":
                    return (long)(new DateTime(leftDateTime.Year, leftDateTime.Month, leftDateTime.Day, leftDateTime.Hour, 0, 0)
                        - new DateTime(rightDateTime.Year, rightDateTime.Month, rightDateTime.Day, rightDateTime.Hour, 0, 0)).TotalHours;
                case "mi":
                    return (long)(new DateTime(leftDateTime.Year, leftDateTime.Month, leftDateTime.Day, leftDateTime.Hour, leftDateTime.Minute, 0)
                        - new DateTime(rightDateTime.Year, rightDateTime.Month, rightDateTime.Day, rightDateTime.Hour, rightDateTime.Minute, 0)).TotalMinutes;
                default:
                    throw new RuntimeException($"Invalid value specified for diffType parameter.  Given: {partString} expected one of the following: y m d h mi", this);
            }
        }

        /// <summary>
        /// Parses the input string and returns a DateTime from the parsed value
        /// </summary>
        /// <param name="date">The value to parse</param>
        /// <param name="useUtc">Whether the result should be in UTC format or not</param>
        /// <returns></returns>
        public DateTime DATEPARSE(object? date, object? useUtc = null)
        {
            bool useUtcBool = false;
            if (useUtc != null)
            {
                useUtcBool = SageValue.ToBoolean(useUtc);
            }

            CultureInfo currentCulture = this._currentCulture;
            DateTimeStyles dateTimeStyles = DateTimeStyles.AssumeLocal;
            if (useUtcBool)
            {
                dateTimeStyles |= DateTimeStyles.AdjustToUniversal;
                return SageValue.ToDateTime(date, currentCulture, dateTimeStyles).UtcDateTime;
            }

            return SageValue.ToDateTime(date, currentCulture, dateTimeStyles).LocalDateTime;
        }
    }
}
