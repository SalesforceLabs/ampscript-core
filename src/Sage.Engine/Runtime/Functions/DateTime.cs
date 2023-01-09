// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Runtime
{
    public partial class RuntimeContext
    {
        /// <summary>
        /// Returns the current system (server) date and time.
        /// Now() is in Central Standard Time (CST) without daylight saving time.
        /// </summary>
        /// <param name="useSendTimeStarted">Determines whether to preserve the email sent time for post-send resolution of Now(). A value of true preserves the email sent time.</param>
        public DateTimeOffset NOW(object? useSendTimeStarted = null)
        {
            if (useSendTimeStarted != null)
            {
                throw new NotImplementedException("useSendTimeStarted is not implemented");
            }

            // TODO: Use CST without daylight savings
            return DateTimeOffset.Now;
        }
    }
}
