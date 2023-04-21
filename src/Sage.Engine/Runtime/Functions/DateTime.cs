// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

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
            DateTimeOffset leftDateTime = SageValue.ToDateTime(end);
            DateTimeOffset rightDateTime = SageValue.ToDateTime(start);
            string? partString = diffType.ToString()?.ToLower();

            switch (partString)
            {
                case "y":
                    return leftDateTime.Year - rightDateTime.Year;
                case "m":
                    return (leftDateTime.Month - rightDateTime.Month) + (12 * (leftDateTime.Year - rightDateTime.Year));
                case "d":
                    return ((long)(leftDateTime - rightDateTime).TotalDays);
                case "h":
                    return ((long)(leftDateTime - rightDateTime).TotalHours);
                case "mi":
                    return ((long)(leftDateTime - rightDateTime).TotalMinutes);
                default:
                    throw new RuntimeException($"Invalid value specified for diffType parameter.  Given: {partString} expected one of the following: y m d h mi", this);

            }
        }
    }
}
