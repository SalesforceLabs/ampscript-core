// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Parameter: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Data
{
    /// <summary>
    /// Builds a <see cref="LookupRequest"/>
    /// </summary>
    public class LookupRequestBuilder
    {
        private readonly string _dataExtension;
        private readonly List<string> _resultAttributes = new();
        private readonly Dictionary<string, string> _attributeConstraints = new();
        private readonly Dictionary<string, Order> _attributeOrderings = new();
        private int _limit;
        private bool _caseSensitive;

        public LookupRequestBuilder(string dataExtension)
        {
            this._dataExtension = dataExtension;
            this._caseSensitive = false;
        }

        public LookupRequestBuilder WithConstraint(string property, string value)
        {
            _attributeConstraints[property] = value;

            return this;
        }

        public LookupRequestBuilder WithReturnedProperty(string property)
        {
            _resultAttributes.Add(property);

            return this;
        }

        public LookupRequestBuilder WithLimit(int inputLimit)
        {
            _limit = inputLimit;

            return this;
        }

        public LookupRequestBuilder WithOrder(string property, Order order)
        {
            _attributeOrderings[property] = order;

            return this;
        }

        public LookupRequestBuilder WithCaseSensitivity(bool caseSensitivity)
        {
            this._caseSensitive = caseSensitivity;

            return this;
        }

        public LookupRequest Build()
        {
            if (_resultAttributes.Count == 0)
            {
                _resultAttributes.Add("*");
            }

            return new LookupRequest(
                _dataExtension,
                _limit,
                this._caseSensitive,
                _resultAttributes,
                _attributeConstraints,
                _attributeOrderings);
        }
    }
}
