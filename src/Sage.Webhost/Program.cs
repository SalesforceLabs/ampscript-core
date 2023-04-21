// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json.Nodes;
using Microsoft.Extensions.FileProviders;
using Sage.Engine;
using Sage.Engine.Runtime;
using StackFrame = Sage.Engine.Runtime.StackFrame;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string assetsPath = Path.Combine(builder.Environment.ContentRootPath, "assets");
if (Path.Exists(assetsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(assetsPath),
        RequestPath = "/assets"
    });
}

var commandLine = new CommandLine()
{
    LaunchCommand =
    {
        Handler = CommandHandler.Create(RunAmpscriptFile)
    },
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
    catch (CompileCodeException compileException)
    {
        subscriberExceptionContext["ExceptionType"] = compileException.GetType().Name;
        subscriberExceptionContext["Message"] = compileException.Message;
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
        var callstack = new JsonArray();
        foreach (StackFrame frame in runtimeException.AmpscriptCallstack)
        {
            JsonNode jsonFrame = new JsonObject();
            jsonFrame["Name"] = frame.Name;
            jsonFrame["CurrentLineNumber"] = frame.CurrentLineNumber;
            jsonFrame["Code"] = GetLinesFromCode(frame.CurrentLineNumber, frame.Code);
            callstack.Add(jsonFrame);
        }

        subscriberExceptionContext["Stack"] = callstack;
        subscriberExceptionContext["ExceptionType"] = runtimeException.GetType().Name;
        subscriberExceptionContext["Message"] = runtimeException.Message;
    }

    return Renderer.Render(new FileInfo(Path.Join(Environment.CurrentDirectory, "Exception.ampscript")), new SubscriberContext(subscriberExceptionContext.ToJsonString()));
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

commandLine.LaunchCommand.Invoke(args);
