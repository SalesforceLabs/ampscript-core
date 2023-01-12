// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// A query parameter ties together Data Extension attributes to an underlying SQLite column, and provides
    /// a mechanism to support safely building dynamic SQL queries.
    /// </summary>
    internal class ParameterizedQueryItem
    {
        /// <summary>
        /// The name of the column as a <see cref="QuotedIdentifier"/>
        /// </summary>
        public QuotedIdentifier QuotedIdentifier
        {
            get;
        }

        /// <summary>
        /// The name of the column, as a parameterized name (aka $name).
        /// </summary>
        public string ParameterizedName
        {
            get;
        }

        /// <summary>
        /// The data for the value of this parameter.
        /// </summary>
        public string Data
        {
            get;
        }

        /// <summary>
        /// Whether or not this field should be used in a case-sensitive manner
        /// </summary>
        public bool CaseSensitive
        {
            get;
        }

        public ParameterizedQueryItem(string parameterName, string data, bool caseSensitive)
        {
            QuotedIdentifier = QuotedIdentifier.Create(parameterName);
            ParameterizedName = $"${QuotedIdentifier.Unquote()}";
            CaseSensitive = caseSensitive;
            Data = data;
        }

        public override string ToString()
        {
            string caseSensitiveQueryPart = string.Empty;
            if (!CaseSensitive)
            {
                caseSensitiveQueryPart = " COLLATE NOCASE";
            }

            return $"{QuotedIdentifier} = {ParameterizedName}{caseSensitiveQueryPart}";
        }
    }
}
