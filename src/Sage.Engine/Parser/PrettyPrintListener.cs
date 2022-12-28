// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Sage.Engine.Parser;

/// <summary>
/// Prints the output of the parse tree in a lisp-like syntax that's human readable
/// </summary>
public class PrettyPrintListener : IParseTreeListener
{
    public StringBuilder Result
    {
        get;
    } = new StringBuilder();

    int _indent;

    public void EnterEveryRule([NotNull] ParserRuleContext ctx)
    {
        Result.AppendLine();
        for (int i = 0; i < _indent; i++)
        {
            Result.Append(' ');
        }
        Result.Append($"({ctx.GetType().Name}");
        _indent += 2;
    }

    public void ExitEveryRule([NotNull] ParserRuleContext ctx)
    {
        _indent -= 2;
        Result.Append(')');
    }

    public void VisitErrorNode(IErrorNode node)
    {
    }

    public void VisitTerminal(ITerminalNode node)
    {
    }
}