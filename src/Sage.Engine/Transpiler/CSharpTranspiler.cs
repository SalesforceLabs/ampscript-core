// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

namespace Sage.Engine.Transpiler;

/// <summary>
/// This is the main entrypoint into the transpilation process.
///
/// It holds all visitors responsible for visiting the parse tree created by ANTLR, as well as
/// the variable helper to get & set variables at scope.
/// </summary>
internal class CSharpTranspiler
{
    internal Runtime Runtime
    {
        get;
    }

    internal BlockVisitor BlockVisitor
    {
        get;
    }

    internal StatementVisitor StatementVisitor
    {
        get;
    }

    internal ExpressionVisitor ExpressionVisitor
    {
        get;
    }

    internal StringVisitor StringVisitor
    {
        get;
    }

    public CSharpTranspiler()
    {
        BlockVisitor = new BlockVisitor(this);
        StatementVisitor = new StatementVisitor(this);
        ExpressionVisitor = new ExpressionVisitor(this);
        StringVisitor = new StringVisitor(this);
        Runtime = new Runtime();
    }
}