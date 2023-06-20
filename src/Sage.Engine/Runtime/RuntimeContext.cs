// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Sage.Engine.Compiler;
using Sage.Engine.Content;
using Sage.Engine.Data;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// The runtime context stores information about this rendering request
    /// </summary>
    /// <remarks>
    /// Things that belong here are variables, details about the subscriber being rendered
    /// or details about the HTTP request if this were in a cloud page context.
    /// </remarks>
    [DebuggerDisplay("Variables and Current Content", Name = "", Type = "")]
    [DebuggerTypeProxy(typeof(RuntimeContextDebugView))]
    public partial class RuntimeContext : IDisposable
    {
        private readonly IContentClient _classicContentClient;
        private readonly IContentClient _contentBuilderContentClient;

        // TODO: Make configurable
        private readonly CultureInfo _currentCulture;
        private readonly TimeZoneInfo _currentTimezone;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly CompilationOptions _rootCompilationOptions;
        private readonly Stack<StackFrame> _stackFrame = new();
        private readonly SubscriberContext _subscriberContext;

        private readonly Dictionary<string, SageVariable> _variables = new();

        public RuntimeContext(
            IServiceProvider provider,
            CompilationOptions rootCompileOptions,
            SubscriberContext? subscriberContext = null)
        {
            _currentCulture = CompatibleGlobalizationSettings.GetCulture("en-US");
            _currentTimezone = TimeZoneInfo.Local;
            Random = new Random();
            _rootCompilationOptions = rootCompileOptions;
            _classicContentClient = provider.GetRequiredService<IClassicContentClient>();
            _contentBuilderContentClient = provider.GetRequiredService<IContentBuilderContentClient>();
            _dataExtensionClient = provider.GetRequiredService<IDataExtensionClient>();
            _dataExtensionClient.ConnectAsync().Wait();

            if (_rootCompilationOptions.InputFile.Directory == null)
            {
                throw new InternalEngineException(
                    $"Directory for the input file ({_rootCompilationOptions.InputFile.FullName}) is unexpectedly not available");
            }

            string subscriberContextFile =
                Path.Combine(_rootCompilationOptions.InputFile.Directory.FullName, "subscriber.json");
            if (subscriberContext != null)
            {
                _subscriberContext = subscriberContext;
            }
            else if (Path.Exists(subscriberContextFile))
            {
                _subscriberContext = new SubscriberContext(File.ReadAllText(subscriberContextFile));
            }
            else
            {
                _subscriberContext = new SubscriberContext(null);
            }

            _stackFrame.Push(new StackFrame(_rootCompilationOptions.GeneratedMethodName,
                _rootCompilationOptions.InputFile));
        }

        public void Dispose()
        {
            _dataExtensionClient.Dispose();
        }

        /// <summary>
        /// Returns the current stack frames for this runtime
        /// </summary>
        internal IEnumerable<StackFrame> GetStackFrames()
        {
            return _stackFrame;
        }

        /// <summary>
        /// Returns the current variables for this runtime
        /// </summary>
        internal IEnumerable<SageVariable> GetVariables()
        {
            return _variables.Values;
        }

        /// <summary>
        /// Returns a variable from the runtime
        /// </summary>
        public SageVariable GetVariable(string name)
        {
            if (!_variables.TryGetValue(name, out SageVariable? result))
            {
                _variables[name] = new SageVariable(name);
                return _variables[name];
            }

            return result;
        }

        /// <summary>
        /// Sets a variable in the runtime
        /// </summary>
        public void SetVariable(string name, object? value)
        {
            SageVariable existingVariable = GetVariable(name);

            existingVariable.Value = value;
        }

        /// <summary>
        /// Adds the data to the output stream
        /// </summary>
        public void Output(object? data)
        {
            _stackFrame.Peek().OutputStream.Append(data);
        }

        /// <summary>
        /// Adds the data to the output stream, with the default line ending
        /// </summary>
        public void OutputLine(object? data)
        {
            _stackFrame.Peek().OutputStream.AppendLine(data?.ToString());
        }

        /// <summary>
        /// Flushes the output stream and returns the built-up data
        /// </summary>
        public string PopContext()
        {
            string results = _stackFrame.Pop().OutputStream.ToString();
            return results;
        }

        /// <summary>
        /// Gets the context ready for a new content stack frame
        /// </summary>
        /// <param name="name">The name of the content</param>
        /// <param name="code">The file where the content exists</param>
        public void PushContext(string name, FileInfo code)
        {
            _stackFrame.Push(new StackFrame(name, code));
        }

        /// <summary>
        /// Gets the context ready for a new content stack frame
        /// </summary>
        /// <param name="name">The name of the content</param>
        /// <param name="code">The TREATASCONTENT code</param>
        public void PushContext(string name, string code)
        {
            _stackFrame.Push(new StackFrame(name, code));
        }

        public void SetCurrentContextLineNumber(int lineNumber)
        {
            _stackFrame.Peek().CurrentLineNumber = lineNumber;
        }

        public IDataExtensionClient GetDataExtensionClient()
        {
            return _dataExtensionClient;
        }

        public IContentClient GetClassicContentClient()
        {
            return _classicContentClient;
        }

        public IContentClient GetContentBuilderContentClient()
        {
            return _contentBuilderContentClient;
        }

        /// <summary>
        /// The subscriber context is the set of attributes that are unique to the
        /// subscriber that this message is being personalized for. This gets a pointer
        /// to that context.
        /// </summary>
        public SubscriberContext GetSubscriberContext()
        {
            return _subscriberContext;
        }

        internal string? CompileAndExecuteEmbeddedCodeAsync(string id, string code)
        {
            CompilerOptionsBuilder fromCodeString =
                new CompilerOptionsBuilder(_rootCompilationOptions).WithSourceCode(id, code);

            return CompileAndExecuteEmbeddedCodeAsync(fromCodeString.Build(), code);
        }

        /// <summary>
        /// Executes embedded code - whether that came from TREATASCONTENT or CONTENT calls.
        /// </summary>
        /// <param name="id">
        /// The identifier of this particular code.  Can be the content ID, or an identifier to the TREATASCONTENT
        /// line of code
        /// </param>
        /// <param name="getCode">
        /// A delegate which produces the code.  This interface is a delegate which allows for future caching
        /// of content retrieval.
        /// </param>
        /// <returns>The result of executing the code</returns>
        internal string? CompileAndExecuteEmbeddedCodeAsync(string id, Func<FileInfo?> getCode)
        {
            FileInfo? code = getCode();
            if (code == null)
            {
                return null;
            }

            CompilerOptionsBuilder fromFileOptions =
                new CompilerOptionsBuilder(_rootCompilationOptions).WithInputFile(code);

            return CompileAndExecuteEmbeddedCodeAsync(fromFileOptions.Build(), code);
        }

        internal string? CompileAndExecuteEmbeddedCodeAsync(CompilationOptions currentOptions, object fileInfoOrString)
        {
            CompileResult compileResult = CSharpCompiler.GenerateAssemblyFromSource(currentOptions);

            string poppedContext;

            if (fileInfoOrString is FileInfo contextFile)
            {
                PushContext(currentOptions.GeneratedMethodName, contextFile);
            }
            else
            {
                PushContext(currentOptions.GeneratedMethodName, fileInfoOrString.ToString() ?? string.Empty);
            }

            try
            {
                compileResult.Execute(this);
            }
            finally
            {
                poppedContext = PopContext();
            }

            return poppedContext;
        }
    }
}
