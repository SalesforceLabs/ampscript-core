// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/output.html
        /// 
        /// Outputs to where this block appears in the content.
        /// </summary>
        public void OUTPUT(object? data)
        {
            Output(data);
        }

        /// <summary>
        /// https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/OutputLine.html
        /// 
        /// Outputs to where this block appears in the content, with a newline
        /// </summary>
        public void OUTPUTLINE(object? data)
        {
            OutputLine(data);
        }

        /// <summary>
        /// https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/v.html
        /// 
        /// Outputs a variable where this appears in the content
        /// </summary>
        public void V(object? data)
        {
            Output(data);
        }
    }
}
