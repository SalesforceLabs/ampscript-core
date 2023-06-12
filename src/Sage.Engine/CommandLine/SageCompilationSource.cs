// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage.Engine.CommandLine
{
    public interface ISageCompilationSource
    {
        Command Command { get; }
    }

    public class SageCompilationSource<CommandT> : ISageCompilationSource where CommandT : Command
    {
        public Command Command { get; private set; }
        public SageCompilationSource(
            CommandT command)
        {
            Command = command;
        }
    }
}
