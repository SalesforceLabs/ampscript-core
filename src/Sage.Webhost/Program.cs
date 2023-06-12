// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.CommandLine;
using Microsoft.Extensions.Options;
using Sage.Engine;
using Sage.Engine.Compiler;
using Sage.Engine.DependencyInjection;
using Sage.Webhost.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
builder.Services.AddSageWebhost(args, builder.Configuration);
WebApplication app = builder.Build();

// If the command lines indicated a Package Manager package, then the package needs extracted in order to operate on it.
// This is only added to the container when the command line arguments specify a packaged item.
PackageExtractor? extractor = app.Services.GetService<PackageExtractor>();
if (extractor != null)
{
    extractor.Extract();
}

app.MapGet("/", () =>
{
    using IServiceScope requestScope = app.Services.CreateScope();

    CompilationOptions options =
        requestScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CompilationOptions>>().Value;

    return Results.Content(
        requestScope.ServiceProvider.GetRequiredService<WebRequestRenderer>().RenderContent(options), "text/html");
});

app.Run();

return 0;