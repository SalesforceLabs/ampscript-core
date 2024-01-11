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
        /// AMPscript supports both "en-us" AND "en_us".  In order to quickly support this lookup, a dictionary of names->cultures is made and populated
        /// on demand.
        /// </summary>
        private static readonly Lazy<Dictionary<string, CultureInfo>> s_cultureLookup = new(CacheCultureNameLookup, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Caches a lookup of name->culture, along with any modifications that are needed to be compatible with the existing language.
        ///
        /// This cache is populated at first access.
        /// </summary>
        private static Dictionary<string, CultureInfo> CacheCultureNameLookup()
        {
            CultureInfo[] validCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var returnDictionary = new Dictionary<string, CultureInfo>(validCultures.Length, StringComparer.InvariantCultureIgnoreCase);
            foreach (CultureInfo culture in validCultures)
            {
                CultureInfo usedCulture = GenerateCompatibleCulture(culture);
                returnDictionary[usedCulture.Name] = usedCulture;

                if (usedCulture.Name.IndexOf("-", StringComparison.Ordinal) > 0)
                {
                    returnDictionary[usedCulture.Name.Replace("-", "_")] = usedCulture;
                }
            }

            return returnDictionary;
        }

        /// <summary>
        /// There will be many differences between linux, mac, windows for globalization settings.
        /// Example, see: https://github.com/dotnet/runtime/issues/18345
        /// In that issue, there is no desire to support common settings between different OSs.
        /// In order to maintain compatibility with existing code, the culture infos are hardcoded.
        /// For now, just support en-US.  In the future, a more robust test strategy and validation needs created.
        /// </summary>
        private static CultureInfo GenerateCompatibleCulture(CultureInfo culture)
        {
            if (culture.Name.ToLower() == "en-us")
            {
                var returnCulture = (CultureInfo)culture.Clone();
                returnCulture.DateTimeFormat.ShortDatePattern = "M/d/yyyy";
                returnCulture.NumberFormat.NumberDecimalDigits = 2;

                return returnCulture;
            }

            return culture;
        }

        /// <summary>
        /// Obtains the compatible CultureInfo from the given culture name
        /// </summary>
        public static CultureInfo GetCulture(string culture)
        {
            return s_cultureLookup.Value[culture];
        }
    }
}