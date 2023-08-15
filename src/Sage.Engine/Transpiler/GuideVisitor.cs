// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Parser;

namespace Sage.Engine.Transpiler;

/// <summary>
/// This holds all GUIDE visitors.
///
/// There may be a time that GUIDE visitors are more complex, but today is not that day for simple GUIDE support found in default content templates.
/// </summary>
internal class GuideVisitor : SageParserBaseVisitor<IEnumerable<StatementSyntax>>
{
    private readonly CSharpTranspiler _transpiler;

    public GuideVisitor(CSharpTranspiler transpiler)
    {
        this._transpiler = transpiler;
    }

    public override IEnumerable<StatementSyntax> VisitGuideSlotTag(SageParser.GuideSlotTagContext context)
    {
        return _transpiler.BlockVisitor.VisitContentBlock(context.contentBlock());
    }

    public override IEnumerable<StatementSyntax> VisitGuideDataTag(SageParser.GuideDataTagContext context)
    {
        return Enumerable.Empty<StatementSyntax>();
    }

    public override IEnumerable<StatementSyntax> VisitGuideBlockTag(SageParser.GuideBlockTagContext context)
    {
        return _transpiler.BlockVisitor.VisitContentBlock(context.contentBlock());
    }
}
