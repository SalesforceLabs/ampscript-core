// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using System.Text.RegularExpressions;

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
        /// Concatenates the strings specified in the arguments. Include as many values as necessary.
        /// </summary>
        /// <param name="strings">The set of strings to concatenate together</param>
        /// <returns>A new string with the values concatenated</returns>
        public string CONCAT(params object[] strings)
        {
            var builder = new StringBuilder();

            foreach (object val in strings)
            {
                builder.Append(val);
            }

            string result = builder.ToString();

            return result;
        }

        /// <summary>
        /// Returns the character position where the <see cref="search"/> variable exists in <see cref="subject"/>.
        /// </summary>
        /// <remarks>The index numbering begins at 1. The search is case-insensitive</remarks>
        /// <param name="subject">The string to search</param>
        /// <param name="search">The string to find</param>
        /// <returns>The position that search exists in subject. If not found, -1 is returned.</returns>
        public int INDEXOF(object subject, object search)
        {
            if (subject == null || search == null)
            {
                return 0;
            }

            string? subjectString = subject.ToString()?.ToLower();
            string? searchString = search.ToString()?.ToLower();

            if (subjectString == null || searchString == null)
            {
                return 0;
            }

            int value = subjectString.IndexOf(searchString);

            // The function returns offset of 1, but IndexOf returns offset of 0.
            return value + 1;
        }

        /// <summary>
        /// Returns the ASCII character for the specified ASCII character code
        /// </summary>
        public string CHAR(object code)
        {
            return CHAR(code, 1);
        }

        /// <summary>
        /// Returns the ASCII character for the specified ASCII character code
        /// </summary>
        public string CHAR(object code, object? repeated)
        {
            int codeInt = SageValue.ToInt(code);

            if (repeated == null || SageValue.TryToInt(repeated, out int repeatedInt) == SageValue.UnboxResult.Fail)
            {
                repeatedInt = 1;
            }

            if (repeatedInt == 1)
            {
                return ((char)codeInt).ToString();
            }

            var builder = new StringBuilder(repeatedInt);

            for (int i = 0; i < repeatedInt; i++)
            {
                builder.Append((char)codeInt);
            }
            string result = builder.ToString();

            return result;
        }

        /// <summary>
        /// If the input evaluates to NULL or empty, it will output `true`, otherwise it outputs `false`.
        /// </summary>
        public bool EMPTY(object input)
        {
            return string.IsNullOrEmpty(input?.ToString());
        }

        /// <summary>
        /// This function capitalizes the first letter in the specified string and any other letters in the string that follow any character other than a letter. It converts all other letters into lowercase.
        /// </summary>
        public string PROPERCASE(object input)
        {
            var newString = new StringBuilder();

            bool shouldCapitalize = true;
            foreach (char charItem in input?.ToString() ?? string.Empty)
            {
                char nextChar = shouldCapitalize ? char.ToUpper(charItem) : char.ToLower(charItem);
                newString.Append(nextChar);

                shouldCapitalize = char.IsWhiteSpace(charItem);
            }

            string result = newString.ToString();

            return result;
        }

        public string SUBSTRING(object subject, object start, object? length = null)
        {
            int startInt = 1;
            if (subject == null)
            {
                return string.Empty;
            }

            if (start != null)
            {
                startInt = SageValue.ToInt(start);
            }

            if (length == null)
            {
                return subject?.ToString()?.Substring(startInt - 1) ?? string.Empty;
            }

            int lengthInt = SageValue.ToInt(length);

            return subject?.ToString()?.Substring(startInt - 1, lengthInt) ?? string.Empty;
        }

        /// <summary>
        /// Returns the number of characters in the specified string.
        /// </summary>
        /// <param name="subject">String to evaluate</param>
        public int LENGTH(object subject)
        {
            return subject?.ToString()?.Length ?? 0;
        }

        /// <summary>
        /// Returns the specified value in all lowercase letters.
        /// </summary>
        /// <param name="subject">Specified string value</param>
        public string LOWERCASE(object subject)
        {
            return subject?.ToString()?.ToLower() ?? string.Empty;
        }

        /// <summary>
        /// Returns the specified value in all uppercase letters.
        /// </summary>
        /// <param name="subject">Specified string value</param>
        public string UPPERCASE(object subject)
        {
            return subject?.ToString()?.ToUpper() ?? string.Empty;
        }

        /// <summary>
        /// Returns the specified value with the leading and trailing whitespace removed
        /// </summary>
        /// <param name="subject">Specified string value</param>
        public string TRIM(object subject)
        {
            return subject?.ToString()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Allows you to use a regular expression to search for information in a string. Use any value from the .NET RegexOptions enumeration, such as IgnoreCase and Multiline.
        /// </summary>
        /// <param name="subject">String to search</param>
        /// <param name="regex">Regular expression to use in the search</param>
        /// <param name="matchGroup">Name or ordinal of the matching group to return</param>
        /// <param name="regexOptions">Repeating string parameter of regular expression options to use</param>
        public string REGEXMATCH(object subject, object regex, object matchGroup, params object[] regexOptions)
        {
            string subjectString = subject?.ToString() ?? string.Empty;
            string regexString = regex?.ToString() ?? string.Empty;
            string matchGroupString = matchGroup?.ToString() ?? string.Empty;

            RegexOptions resolvedOptions = RegexOptions.None;
            foreach (object? option in regexOptions)
            {
                string thisOption = option?.ToString() ?? string.Empty;

                if (Enum.TryParse(typeof(RegexOptions), thisOption, true, out object? resolvedOption))
                {
                    if (resolvedOptions == RegexOptions.None)
                    {
                        resolvedOptions = (RegexOptions)resolvedOption;
                    }
                    else
                    {
                        resolvedOptions |= (RegexOptions)resolvedOption;
                    }
                }
            }

            Match match = Regex.Match(subjectString, regexString, resolvedOptions);

            // Get the result using either the group number or the group name.
            string result;
            if (int.TryParse(matchGroupString, out int groupNum))
            {
                result = match.Groups[groupNum].Value;
            }
            else
            {
                result = match.Groups[matchGroupString].Value;
            }

            return result;
        }

        /// <summary>
        /// Replaces the first string value with the second string value anywhere it is found in the variable.
        /// </summary>
        /// <remarks>The search is string-insensitive, the result is the original case</remarks>
        /// <param name="subject">Variable with value to replace</param>
        /// <param name="search">String to replace</param>
        /// <param name="replacement">String used as replacement</param>
        public string REPLACE(object subject, object search, object replacement)
        {
            string subjectString = subject?.ToString() ?? string.Empty;
            string searchString = (search?.ToString() ?? string.Empty);
            string replacementString = replacement?.ToString() ?? string.Empty;

            return ReplaceWithCaseInsensitiveSearch(subjectString, replacementString, new[] { searchString });
        }

        /// <summary>
        /// Replaces the first string value with the second string value anywhere it is found in the variable.
        /// </summary>
        /// <remarks>The search is string-insensitive, the result is the original case</remarks>
        /// <param name="subject">String with value to replace</param>
        /// <param name="replace">The string t</param>
        /// <param name="searches">Strings to find in the original subject string</param>
        public string REPLACELIST(object subject, object replace, params object[] searches)
        {
            string subjectString = subject?.ToString() ?? string.Empty;
            string replaceString = (replace?.ToString() ?? string.Empty);
            string[] searchStrings = searches.Select(r => r?.ToString() ?? string.Empty).ToArray();

            return ReplaceWithCaseInsensitiveSearch(subjectString, replaceString, searchStrings);
        }

        /// <summary>
        /// Replaces the passed in string with the replacement string in a case-insensitive manner.
        /// </summary>
        /// <param name="originalString">The original string with the correct case to return,</param>
        /// <param name="replacement">The string to be replaced when the search item is found</param>
        /// <param name="searches">Strings to find in the original string</param>
        /// <returns></returns>
        private string ReplaceWithCaseInsensitiveSearch(string originalString, string replacement, string[] searches)
        {
            // Do a case insensitive replace while preserving case
            string originalStringLower = originalString.ToLower();

            foreach (string searchString in searches)
            {
                var thisSearchBuilder = new StringBuilder(originalStringLower.Length + replacement.Length);
                string searchLower = searchString.ToLower();
                if (searchLower == string.Empty)
                {
                    continue;
                }

                int findIndex = originalStringLower.IndexOf(searchLower);
                if (findIndex < 0)
                {
                    continue;
                }

                int copyIndex = 0;
                while (findIndex >= 0)
                {
                    if (copyIndex < findIndex)
                    {
                        thisSearchBuilder.Append(originalString.Substring(copyIndex, findIndex - copyIndex));
                    }

                    thisSearchBuilder.Append(replacement);
                    copyIndex = findIndex + searchLower.Length;
                    findIndex = originalStringLower.IndexOf(searchLower, (findIndex + searchLower.Length));
                }
                if (copyIndex < originalStringLower.Length)
                {
                    thisSearchBuilder.Append(originalString.Substring(copyIndex));
                }

                originalString = thisSearchBuilder.ToString();
            }

            return originalString;
        }
    }
}
