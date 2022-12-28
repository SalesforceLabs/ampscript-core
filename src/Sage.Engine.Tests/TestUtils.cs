// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Sage.Engine.Parser;

namespace Sage.Engine.Tests;

public static class TestUtils
{
    public static EngineParseTestResult GetParseResultFromTest(CorpusData test)
    {
        EngineParseTestResult result;

        var parseTree = AntlrParser.Parse(test.Code, null, null);
        var serializedTree = AntlrParser.SerializeTree(parseTree);
        result = new EngineParseTestResult(serializedTree);

        File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.Name}_expected.txt"),
            test.ParseTree);
        File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.Name}_actual.txt"),
            result.ParseTree);

        return result;
    }
}