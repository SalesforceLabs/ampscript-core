// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net.Mime;
using Microsoft.Extensions.FileProviders;
using Sage.Engine;

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
        Handler = CommandHandler.Create(RunMain)
    },
};

int RunMain(string source, bool debug, IConsole console)
{
    string fullPath = source;
    if (!Path.IsPathRooted(source))
    {
        fullPath = Path.Join(Environment.CurrentDirectory, source);
    }

    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Cannot find {Path.Join(Environment.CurrentDirectory, fullPath)}");
    }

    app.MapGet("/", () => Results.Content(Renderer.Render(fullPath), MediaTypeNames.Text.Html));

    app.Run();
    return 0;
}

commandLine.LaunchCommand.Invoke(args);
