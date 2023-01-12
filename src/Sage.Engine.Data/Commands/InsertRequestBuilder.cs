// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-QuotedIdentifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data
{
    /// <summary>
    /// Builds an <see cref="InsertRequest"/>
    /// </summary>
    public class InsertRequestBuilder
    {
        private readonly string _dataExtension;
        private readonly Dictionary<string, string> _attributeData = new();

        public InsertRequestBuilder(string dataExtension)
        {
            this._dataExtension = dataExtension;
        }

        public InsertRequestBuilder WithAttribute(string attributeName, string attributeValue)
        {
            _attributeData[attributeName] = attributeValue;

            return this;
        }

        public InsertRequest Build()
        {
            return new InsertRequest(_dataExtension, _attributeData);
        }
    }
}
