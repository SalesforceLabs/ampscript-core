// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using Sage.Engine.Data;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// The runtime context stores information about this rendering request
    /// </summary>
    /// <remarks>
    /// Things that belong here are variables, details about the subscriber being rendered
    /// or details about the HTTP request if this were in a cloud page context.
    /// </remarks>
    public partial class RuntimeContext : IDisposable
    {
        readonly StringBuilder _outputStream = new();
        private readonly Dictionary<string, object?> _variables = new();
        private IDataExtensionClient _dataExtensionClient;

        public RuntimeContext()
        {
            this.Random = new Random();
            _dataExtensionClient = DataExtensionClientFactory.CreateInMemoryDataExtensions().Result;
        }

        /// <summary>
        /// Returns a variable from the runtime
        /// </summary>
        public object? GetVariable(string name)
        {
            if (!_variables.TryGetValue(name, out SageVariable? result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Sets a variable in the runtime
        /// </summary>
        public void SetVariable(string name, object? value)
        {
            SageVariable existingVariable = GetVariable(name) ?? new SageVariable(name);

            existingVariable.Value = value;
        }

        /// <summary>
        /// Adds the data to the output stream
        /// </summary>
        public void Output(object? data)
        {
            _outputStream.Append(data?.ToString());
        }

        /// <summary>
        /// Adds the data to the output stream, with the default line ending
        /// </summary>
        public void OutputLine(object? data)
        {
            _outputStream.AppendLine(data?.ToString());
        }

        /// <summary>
        /// Flushes the output stream and returns the built-up data
        /// </summary>
        public string FlushOutputStream()
        {
            string results = _outputStream.ToString();
            _outputStream.Clear();
            return results;
        }

        public IDataExtensionClient GetDataExtensionClient()
        {
            return _dataExtensionClient;
        }

        /// <summary>
        /// The subscriber context is the set of attributes that are unique to the
        /// subscriber that this message is being personalized for. This gets a pointer
        /// to that context.
        /// </summary>
        public SubscriberContext GetSubscriberContext()
        {
            return _subscriberContext;
        }
        public void Dispose()
        {
            _dataExtensionClient.Dispose();
        }
    }
}
