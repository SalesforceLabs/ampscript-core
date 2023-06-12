// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.Json.Nodes;
using Sage.Engine.Compiler;
using Sage.Engine.Runtime;

namespace Sage.Engine
{
    /// <summary>
    /// Renders an exception that occurs using the exception template file
    /// </summary>
    public class ExceptionPageRenderer
    {
        private readonly FileInfo _pathToExceptionHtml;
        private readonly Renderer _renderer;
        private readonly IServiceProvider _services;

        public ExceptionPageRenderer(
            IServiceProvider services,
            Renderer renderer)
        {
            _services = services;
            _renderer = renderer;
            _pathToExceptionHtml =
                new FileInfo(Path.Join(Path.GetDirectoryName(typeof(RuntimeContext).Assembly.Location),
                    "Exception.ampscript"));
        }

        public string Render(GenerateCodeException generateCodeException, CompilationOptions options)
        {
            JsonNode subscriberExceptionContext = new JsonObject();
            subscriberExceptionContext["ExceptionType"] = generateCodeException.GetType().Name;
            subscriberExceptionContext["Message"] = generateCodeException.Message;
            subscriberExceptionContext["Stack"] = new JsonArray();

            options.InputFile = _pathToExceptionHtml;

            return _renderer.Render(
                options,
                new SubscriberContext(subscriberExceptionContext.ToJsonString()));
        }

        public string Render(RuntimeException runtimeException, CompilationOptions options)
        {
            JsonNode subscriberExceptionContext = new JsonObject();
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
                jsonFrame["Code"] = GetLinesFromCode(frame.CurrentLineNumber,
                    frame.CodeFromFile ?? throw new FileNotFoundException());
                callstack.Add(jsonFrame);
            }

            subscriberExceptionContext["Stack"] = callstack;
            subscriberExceptionContext["ExceptionType"] = runtimeException.GetType().Name;
            subscriberExceptionContext["Message"] = runtimeException.Message;

            options.InputFile = _pathToExceptionHtml;
            return _renderer.Render(
                options,
                new SubscriberContext(subscriberExceptionContext.ToJsonString()));
        }

        /// <summary>
        /// Gets lines of code near the given line number
        /// </summary>
        private string GetLinesFromCode(int lineNumber, FileInfo code)
        {
            string[] lines = File.ReadAllLines(code.FullName);

            int start = Math.Max(lineNumber - 3, 0);
            int end = Math.Min(lineNumber + 3, lines.Length);
            return string.Join(Environment.NewLine, lines[new Range(start, end)]);
        }
    }
}
