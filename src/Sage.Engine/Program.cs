// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using System.CommandLine.IO;
using Microsoft.CodeAnalysis;
using Sage.Engine;
using Sage.Engine.Compiler;
using CommandHandler = System.CommandLine.NamingConventionBinder.CommandHandler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

int RunMain(FileInfo source, bool debug, bool execute, IConsole console)
{
    DirectoryInfo tempPath = Directory.CreateTempSubdirectory("Sage");

    OptimizationLevel optimizeLevel = debug ? OptimizationLevel.Debug : OptimizationLevel.Release;

    CompilationOptions options = new CompilerOptionsBuilder().WithInputFile(source)
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
        console.Error.WriteLine(e.ToString());
        return 0;
    }
    catch (Exception e)
    {
        console.Error.WriteLine(e.ToString());
        return -1;
    }

    return 0;
}

var commandLine = new CommandLine
{
    LaunchCommand =
    {
        Handler = CommandHandler.Create(
            (FileInfo source, bool debug, IConsole console) => RunMain(source, debug, true, console))
    },
    CompileCommand =
    {
        Handler = CommandHandler.Create(
            (FileInfo source, bool debug, IConsole console) => RunMain(source, debug, false, console))
    }
};

RootCommand rootCommand = new("Compile or execute AMPscript code");
rootCommand.AddCommand(commandLine.LaunchCommand);
rootCommand.AddCommand(commandLine.CompileCommand);
rootCommand.Invoke(args);