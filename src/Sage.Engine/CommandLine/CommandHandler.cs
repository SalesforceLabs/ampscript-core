// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Reflection.Metadata;
using Sage.Engine.Compiler;

namespace Sage.Engine
{
    public class CommandHandler<TOption>
    {
        private readonly Action<TOption> _action;

        internal CommandHandler(Action<TOption> action)
        {
            _action = action;
        }

        public void Handle(TOption option)
        {
            _action(option);
        }
    }
}
