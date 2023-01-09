// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.CodeAnalysis;
using Sage.Engine.Compiler;
using CommandHandler = System.CommandLine.NamingConventionBinder.CommandHandler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

Argument<string> sourceFile = new Argument<string>(name: "--source", description: "Path to the AMPscript program to debug").LegalFilePathsOnly();
Option<bool> debugOption = new(new[] { "--debug", "-d" }, "Whether or not to build debug information and debug the output");

RootCommand rootCommand = new(description: "Run an AMPscript program under the debugger")
{
    sourceFile,
    debugOption
};

rootCommand.Handler = CommandHandler.Create((string source, bool debug, IConsole console) =>
{
    DirectoryInfo tempPath = Directory.CreateTempSubdirectory("Sage");

    var optimizeLevel = debug ? OptimizationLevel.Debug : OptimizationLevel.Release;

    CompilationOptions options = new CompilerOptionsBuilder()
        .WithInputFile(new FileInfo(source))
        .WithOptimizationLevel(optimizeLevel)
        .Build();

    try
    {
        console.Write(CSharpCompiler.CompileAndExecute(options));
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e);
        return -1;
    }

    return 0;
});

rootCommand.Invoke(args);