// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    return Results.Content(Sage.Engine.Renderer.Render("Page.ampscript"), MediaTypeNames.Text.Html);
});

app.Run();
