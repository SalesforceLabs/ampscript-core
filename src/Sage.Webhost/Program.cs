// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Net.Mime;
using Microsoft.Extensions.FileProviders;

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

app.MapGet("/{*page}", (string? page) =>
{
    string resolvedPage = page;
    if (!File.Exists(page))
    {
        resolvedPage = "Index.ampscript";
    }

    return Results.Content(Sage.Engine.Renderer.Render(resolvedPage), MediaTypeNames.Text.Html);
});

app.Run();
