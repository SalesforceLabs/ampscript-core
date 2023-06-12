// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Runtime;

namespace Sage.Engine.Tests.Functions
{
    /// <summary>
    /// Base class for testing functions
    /// </summary>
    [TestFixture]
    internal class FunctionTestBase : SageTest
    {
        protected RuntimeContext _runtimeContext;

        [SetUp]
        public void TestSetup()
        {
            _runtimeContext = new RuntimeContext(_serviceProvider, TestCompilationOptions());
        }
    }
}
