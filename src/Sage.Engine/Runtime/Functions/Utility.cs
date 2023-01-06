// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Outputs to where this block appears in the content.
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public void OUTPUT(object? data)
        {
            Output(data);
        }

        /// <summary>
        /// Outputs to where this block appears in the content, with a newline
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public void OUTPUTLINE(object? data)
        {
            OutputLine(data);
        }

        /// <summary>
        /// Outputs a variable where this appears in the content
        /// </summary>
        /// <param name="data">The data to add to the output stream</param>
        public void V(object? data)
        {
            Output(data);
        }
    }
}
