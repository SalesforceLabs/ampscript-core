// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.Extensions.Options;
using Sage.Engine.Compiler;
using Sage.Engine.Runtime;
using Sage.Webhost;

namespace Sage.Engine
{
    /// <summary>
    ///     Renders a request for the web.
    ///     This also handles what happens if there is an error - it will show the error page if configured to do so.
    /// </summary>
    public class WebRequestRenderer
    {
        private readonly SageWebhostOptions _config;
        private readonly IServiceProvider _provider;
        private readonly Renderer _renderer;

        public WebRequestRenderer(
            IServiceProvider serviceProvider,
            Renderer renderer,
            IOptionsSnapshot<SageWebhostOptions> configuration)
        {
            _provider = serviceProvider;
            _renderer = renderer;
            _config = configuration.Value;
        }

        public string RenderContent(CompilationOptions inputOption)
        {
            try
            {
                SubscriberContext context = null;
                string subscriberContextFile = Path.Combine(Path.GetDirectoryName(inputOption.Content.Location), "subscriber.json");
                if (Path.Exists(subscriberContextFile))
                {
                    context = new SubscriberContext(File.ReadAllText(subscriberContextFile));
                }

                return _renderer.Render(inputOption, context);
            }
            catch (GenerateCodeException compileException)
            {
                if (!_config.CustomExceptionPage)
                {
                    throw;
                }

                using IServiceScope exceptionScope = _provider.CreateScope();

                CompilationOptions exceptionOptions =
                    exceptionScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CompilationOptions>>().Value;

                return exceptionScope.ServiceProvider.GetRequiredService<ExceptionPageRenderer>()
                    .Render(compileException, exceptionOptions);
            }
            catch (RuntimeException runtimeException)
            {
                if (!_config.CustomExceptionPage)
                {
                    throw;
                }

                using IServiceScope exceptionScope = _provider.CreateScope();

                CompilationOptions exceptionOptions =
                    exceptionScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CompilationOptions>>().Value;

                return exceptionScope.ServiceProvider.GetRequiredService<ExceptionPageRenderer>()
                    .Render(runtimeException, exceptionOptions);
            }
        }
    }
}
