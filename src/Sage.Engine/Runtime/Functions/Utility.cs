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

            CultureInfo cultureInfo = null;
            if (culture != null)
            {
                string? cultureString = culture.ToString();
                if (cultureString != null)
                {
                    cultureInfo = new CultureInfo(cultureString);
                }
            }

            cultureInfo ??= CultureInfo.CurrentCulture;

            // !Backward compatibility!
            // If converting fails, return the source.
            try
            {
                if (string.Compare("date", compareType, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var dto = SageValue.ToDateTime(subject.ToString());
                    return SageValue.ToString(dto, formatString, cultureInfo);
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
        public void V(object data)
        {
            Output(data);
        }
    }
}
