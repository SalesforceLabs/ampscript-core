﻿// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Antlr4.Runtime.Tree;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.Compiler;
using Sage.Engine.Parser;
using Sage.Engine.Runtime;

namespace Sage.Engine.Tests;

public static class TestUtils
{
    /// <summary>
    /// Executes the parser and returns an expected parse tree result from a corpus test
    /// </summary>
    public static ParserTestResult GetParseResultFromTest(CorpusData test)
    {
        ParserTestResult result;

        IParseTree parseTree = AntlrParser.Parse(test.Code, Console.Out, Console.Error);
        string serializedTree = AntlrParser.SerializeTree(parseTree).ReplaceLineEndings("\n");
        result = new ParserTestResult(serializedTree);

        File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.FileFriendlyName}_expected.txt"),
            test.ParseTree);
        File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.FileFriendlyName}_actual.txt"),
            result.ParseTree);

        return result;
    }

    private static RuntimeContext GetTestRuntimeContext(IServiceProvider serviceProvider, CompilationOptions options, CorpusData test)
    {
        return new RuntimeContext(
            serviceProvider,
            options,
            test.SubscriberContext);
    }
    

    /// <summary>
    /// Uses Handlebars to compile the content instead of the AMPscript compiler
    /// </summary>
    public static EngineTestResult GetOutputFromHandlebarTest(IServiceProvider serviceProvider, CorpusData test)
    {
        CompilationOptions options = new CompilerOptionsBuilder()
            .WithContent(new EmbeddedContent(test.Code, test.FileFriendlyName, test.FileFriendlyName, 1, ContentType.Handlebars))
            .Build();
 
        try
        {
            string result = serviceProvider.GetService<ICompiler>()
                !.Compile(options, GetTestRuntimeContext(serviceProvider, options, test), test.SubscriberContext);
            return new EngineTestResult(result.ReplaceLineEndings("\n").Trim());
        }
        catch (Exception)
        {
            return new EngineTestResult("!");
        }
    }

    /// <summary>
    /// Executes the engine and gets the expected result from the engine
    /// </summary>
    public static EngineTestResult GetOutputFromTest(IServiceProvider serviceProvider, CorpusData test)
    {
        CompilationOptions options = new CompilerOptionsBuilder()
            .WithContent(new EmbeddedContent(test.Code, test.FileFriendlyName, test.FileFriendlyName, 1, ContentType.AMPscript))
        .Build();

        try
        {
            var engineResult = new EngineTestResult(CSharpCompiler.CompileAndExecute(options, GetTestRuntimeContext(serviceProvider, options, test), out CompileResult compileResult).ReplaceLineEndings("\n").Trim());

            File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.FileFriendlyName}_transpiled.cs"), compileResult.TranspiledSource);

            return engineResult;
        }
        catch (CompileCodeException e)
        {
            File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{test.FileFriendlyName}_transpiled.cs"), e.CompileResult.TranspiledSource);

            Assert.Fail(e.ToString());
            throw;
        }
        catch (Exception)
        {
            return new EngineTestResult("!");
        }
    }
}
