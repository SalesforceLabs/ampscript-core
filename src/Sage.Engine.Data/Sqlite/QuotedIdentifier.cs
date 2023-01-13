// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data.Sqlite
{
    /// <summary>
    /// Represents a column in SQLite in a strongly-typed manner, quoted with [] surrounding it.
    /// The name will also normalized to UPPERCASE.
    /// </summary>
    internal readonly struct QuotedIdentifier
    {
        private readonly string _name;

        private QuotedIdentifier(string name)
        {
            if (name == "*")
            {
                _name = name;
            }
            else
            {
                _name = $"[{name.ToUpperInvariant().Replace("]", "]]")}]";
            }
        }

        /// <summary>
        /// Removes the [] and returns the underlying string
        /// </summary>
        private static string Unquote(string objectName)
        {
            if (objectName.Length < 2)
            {
                return objectName;
            }

            if (objectName.StartsWith("[", StringComparison.Ordinal) && objectName.EndsWith("]", StringComparison.Ordinal))
            {
                return objectName.Substring(1, objectName.Length - 2).Replace("]]", "]");
            }

            return objectName;
        }

        /// <summary>
        /// Creates a new <see cref="QuotedIdentifier"/>. If the input was already quoted, it maintains the quoting.
        /// If the string was not quoted, it will be quoted.
        /// </summary>
        internal static QuotedIdentifier Create(string objectName)
        {
            string unquotedValue = Unquote(objectName);
            var roundTrip = new QuotedIdentifier(unquotedValue);

            if (string.Compare(roundTrip._name, objectName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return roundTrip;
            }

            return new QuotedIdentifier(objectName);
        }

        /// <summary>
        /// Returns the underlying string without the [] quoting.
        /// </summary>
        public string Unquote()
        {
            return Unquote(_name);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
