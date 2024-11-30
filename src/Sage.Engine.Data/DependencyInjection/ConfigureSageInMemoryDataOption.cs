// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.Options;
using Sage.Engine.Extensions;

namespace Sage.Engine.Data.Sqlite
{
    public class ConfigureSageInMemoryDataOption : IConfigureOptions<SageInMemoryDataOption>
    {
        private readonly IOptions<SageOptions> _sageOptions;

        public ConfigureSageInMemoryDataOption(
            IOptions<SageOptions> sageOptions)
        {
            _sageOptions = sageOptions;
        }

        public void Configure(SageInMemoryDataOption options)
        {
            if (_sageOptions.Value.WorkingPath != null)
            {
                options.DataExtensionDirectory = _sageOptions.Value.WorkingPath.AppendDirectory("DataExtensions");
                return;
            }

            options.DataExtensionDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).AppendDirectory("DataExtensions");
        }
    }
}
