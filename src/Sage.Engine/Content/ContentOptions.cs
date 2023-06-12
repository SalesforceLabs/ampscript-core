// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;

namespace Sage.Engine.Compiler
{
    public class ContentOptions
    {
        public required DirectoryInfo InputDirectory { get; set; }
        public required DirectoryInfo OutputDirectory { get; set; }
    }
}
