// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;

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
        /// Returns the character position where the <see cref="search"/> variable exists in <see cref="subject"/>
        /// </summary>
        /// <remarks>The index numbering begins at 1</remarks>
        /// <param name="subject">The string to search</param>
        /// <param name="search">The string to find</param>
        /// <returns>The position that search exists in subject. If not found, -1 is returned.</returns>
        public int INDEXOF(object? subject, object? search)
        {
            if (subject == null || search == null)
            {
                return -1;
            }

            string? subjectString = subject.ToString();
            string? searchString = search.ToString();

            if (subjectString == null || searchString == null)
            {
                return -1;
            }

            int value = subjectString.IndexOf(searchString);

            if (value == -1)
            {
                return value;
            }

            // The function returns offset of 1, but IndexOf returns offset of 0.
            return value + 1;
        }
    }
}
