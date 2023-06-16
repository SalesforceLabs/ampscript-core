// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Globalization;

namespace Sage.Engine.Runtime
{
    public static class CompatibleGlobalizationSettings
    {
        /// <summary>
        /// There will be many differences between linux, mac, windows for globalization settings.
        /// Example, see: https://github.com/dotnet/runtime/issues/18345
        /// In that issue, there is no desire to support common settings between different OSs.
        /// In order to maintain compatibility with existing code, the culture infos are hardcoded.
        /// For now, just support en-US.  In the future, a more robust test stratgy and validation needs created.
        /// </summary>
        public static CultureInfo GetCulture(string culture)
        {
            CultureInfo returnCulture = (CultureInfo)(new CultureInfo(culture, false)).Clone();

            if (culture.ToLower() == "en-us")
            {
                returnCulture.DateTimeFormat.ShortDatePattern = "M/d/yyyy";
            }

            return returnCulture;
        }
    }
}