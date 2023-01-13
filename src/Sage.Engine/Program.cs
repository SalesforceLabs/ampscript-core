// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.CodeAnalysis;
using Sage.Engine;
using Sage.Engine.Compiler;
using CommandHandler = System.CommandLine.NamingConventionBinder.CommandHandler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

Argument<string> sourceFile = new Argument<string>(name: "--source", description: "Path to the AMPscript program to debug").LegalFilePathsOnly();
Option<bool> debugOption = new(new[] { "--debug", "-d" }, "Whether or not to build debug information and debug the output");

Command runCommand = new("run", description: "Execute AMPscript and write the results to stdout")
{
    sourceFile,
    debugOption
};

Command compileCommand = new("compile", description: "Compile AMPscript and write any errors to STDOUT")
{
    sourceFile,
    debugOption
};

RootCommand rootCommand = new("Compile or execute AMPscript code");
rootCommand.AddCommand(runCommand);
rootCommand.AddCommand(compileCommand);

var commandHandler = (string source, bool debug, bool execute, IConsole console) =>
{
    DirectoryInfo tempPath = Directory.CreateTempSubdirectory("Sage");

    var optimizeLevel = debug ? OptimizationLevel.Debug : OptimizationLevel.Release;

    CompilationOptions options = new CompilerOptionsBuilder()
        .WithInputFile(new FileInfo(source))
        .WithOptimizationLevel(optimizeLevel)
        .Build();

    try
    {
        if (execute)
        {
            console.Write(CSharpCompiler.CompileAndExecute(options));
        }
        else
        {
            CompileResult result = CSharpCompiler.GenerateAssemblyFromSource(options);

            if (!result.EmitResult.Success)
            {
                Console.Error.WriteLine(string.Join("\n", result.EmitResult.Diagnostics));
            }
        }
    }
    catch (CompileCodeException e)
    {
        Console.Error.WriteLine(e);
        return 0;
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e);
        return -1;
    }

    return 0;
};

runCommand.Handler = CommandHandler.Create((string source, bool debug, IConsole console) => commandHandler(source, debug, true, console));
compileCommand.Handler = CommandHandler.Create((string source, bool debug, IConsole console) => commandHandler(source, debug, false, console));

rootCommand.Invoke(args);