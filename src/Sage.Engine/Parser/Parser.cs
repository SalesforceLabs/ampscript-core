// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sage.Engine.Parser;

public static class AntlrParser
{
    public static IParseTree Parse(string code, TextWriter outputStream, TextWriter errorStream)
    {
        var stream = new AntlrInputStream(code);
        var lexer = new SageLexer(stream, outputStream, errorStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new SageParser(tokens, outputStream, errorStream) { BuildParseTree = true };

        return parser.contentBlockFile();
    }

    public static string SerializeTree(IParseTree tree)
    {
        var prettyPrinter = new PrettyPrintListener();
        ParseTreeWalker.Default.Walk(prettyPrinter, tree);
        return prettyPrinter.Result.ToString().Trim();
    }
}
