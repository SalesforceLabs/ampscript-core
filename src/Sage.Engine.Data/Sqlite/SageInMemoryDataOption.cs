// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage.Engine.Data.Sqlite
{
    public class SageInMemoryDataOption
    {
        public required string ConnectionString { get; set; }
        public required DirectoryInfo DataExtensionDirectory { get; set; }
    }
}
