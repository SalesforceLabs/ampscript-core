// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.FileProviders;
using Sage.Engine;
using Sage.Engine.Runtime;
using Sage.PackageManager;
using Sage.Webhost;
using StackFrame = Sage.Engine.Runtime.StackFrame;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Configuration config = new Configuration(false);
app.Configuration.GetSection("Configuration").Bind(config);

var commandLine = new CommandLine()
{
    Ampscript =
    {
        Handler = CommandHandler.Create(RunAmpscriptFile)
    },
    PackageJson =
    {
        Handler = CommandHandler.Create(RunPackageJson)
    },
    PackageZip =
    {
        Handler = CommandHandler.Create(RunPackageZip)
    }
};

/// <summary>
/// Attempts to render the given content.
///
/// If rendering fails due to content issues, it renders the "Exception.ampscript" exception handler page.
/// </summary>
string RenderContent(FileInfo contentPath)
{
    JsonNode subscriberExceptionContext = new JsonObject();
    try
    {
        return Renderer.Render(contentPath);
    }
    catch (GenerateCodeException compileException)
    {
        if (!config.CustomExceptionPage)
        {
            throw;
        }
        subscriberExceptionContext["ExceptionType"] = compileException.GetType().Name;
        subscriberExceptionContext["Message"] = compileException.Message;
        subscriberExceptionContext["Stack"] = new JsonArray();
    }
    catch (RuntimeException runtimeException)
    {
        // Creates a JSON array like the following:
        // [
        //   {
        //     "Name": Name_Of_Content,
        //     "CurrentLineNumber": Line_Number_In_Content
        //     "Code": Code_Near_That_Line_Number
        //   },
        //   { ... },
        // ]
        if (!config.CustomExceptionPage)
        {
            throw;
        }

        var callstack = new JsonArray();
        foreach (StackFrame frame in runtimeException.AmpscriptCallstack)
        {
            JsonNode jsonFrame = new JsonObject();
            jsonFrame["Name"] = frame.Name;
            jsonFrame["CurrentLineNumber"] = frame.CurrentLineNumber;
            jsonFrame["Code"] = GetLinesFromCode(frame.CurrentLineNumber, frame.CodeFromFile ?? throw new FileNotFoundException());
            callstack.Add(jsonFrame);
        }

        subscriberExceptionContext["Stack"] = callstack;
        subscriberExceptionContext["ExceptionType"] = runtimeException.GetType().Name;
        subscriberExceptionContext["Message"] = runtimeException.Message;
    }

    string pathToExceptionHtml = Path.Join(Path.GetDirectoryName(typeof(RuntimeContext).Assembly.Location), "Exception.ampscript");

    return Renderer.Render(new FileInfo(pathToExceptionHtml), new SubscriberContext(subscriberExceptionContext.ToJsonString()));
}

int RunAmpscriptFile(FileInfo source, IConsole console)
{
    if (!source.Exists)
    {
        throw new FileNotFoundException($"Cannot find {source.FullName}");
    }

    app.MapGet("/", () => Results.Content(RenderContent(source), MediaTypeNames.Text.Html));

    app.Run();
    return 0;
}

int RunPackage(PackageGraph graph, CommandLine.EntityType type, string sourceId, DirectoryInfo outputDirectory)
{
    if (outputDirectory == null)
    {
        outputDirectory = new DirectoryInfo("out");
    }

    if (!outputDirectory.Exists)
    {
        Directory.CreateDirectory(outputDirectory.FullName);
    }

    FileInfo flattenedContentPath = new FileInfo(Path.Join(outputDirectory.FullName, "sourceId.ampscript"));

    switch (type)
    {
        case CommandLine.EntityType.Asset:
            Asset? asset = graph.GetAsset(int.Parse(sourceId));

            if (asset == null)
            {
                Console.Error.WriteLine($"Cannot find asset {sourceId}");
                return -1;
            }

            File.WriteAllText(flattenedContentPath.FullName, asset.Compile());
            break;
    }

    app.MapGet("/", () => Results.Content(RenderContent(flattenedContentPath), MediaTypeNames.Text.Html));

    app.Run();

    return 0;
}

int RunPackageZip(DirectoryInfo packageDir, CommandLine.EntityType type, string sourceId, DirectoryInfo outputDirectory, IConsole console)
{
    return RunPackage(PackageGraph.FromZip(packageDir), type, sourceId, outputDirectory);
}

int RunPackageJson(FileInfo source, CommandLine.EntityType type, string sourceId, DirectoryInfo outputDirectory, IConsole console)
{
    return RunPackage(PackageGraph.FromJson(source), type, sourceId, outputDirectory);
}

/// <summary>
/// Gets lines of code near the given line number
/// </summary>
string GetLinesFromCode(int lineNumber, FileInfo code)
{
    string[] lines = File.ReadAllLines(code.FullName);

    int start = Math.Max(lineNumber - 3, 0);
    int end = Math.Min(lineNumber + 3, lines.Length);
    return string.Join(Environment.NewLine, lines[new Range(start, end)]);
}

commandLine.LaunchCommand.Invoke(args);
