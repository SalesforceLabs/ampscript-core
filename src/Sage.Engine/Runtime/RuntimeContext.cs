// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Sage.Engine.Content;
using Sage.Engine.Data;
using Sage.Engine.Compiler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;
using System.Diagnostics;
using System.Globalization;

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
        private readonly Stack<StackFrame> _stackFrame = new();

        private readonly Dictionary<string, SageVariable> _variables = new();
        private CompilationOptions? _rootCompilationOptions;
        private IDataExtensionClient _dataExtensionClient;
        private IContentClient _classicContentClient;
        private IContentClient _contentBuilderContentClient;
        private SubscriberContext _subscriberContext;

        // TODO: Make configurable
        private CultureInfo _currentCulture = CultureInfo.InvariantCulture;

        private DirectoryInfo WorkingDirectory => _rootCompilationOptions?.InputFile.Directory ?? new DirectoryInfo(Environment.CurrentDirectory);

        public RuntimeContext(
            CompilationOptions rootCompilationOptions,
            IDataExtensionClient dataExtensionClient,
            SubscriberContext? subscriberContext)
        {
            _rootCompilationOptions = rootCompilationOptions;
            _subscriberContext = subscriberContext ?? new SubscriberContext(null);
            _dataExtensionClient = dataExtensionClient;
            _classicContentClient = ContentClientFactory.CreateLocalDiskContentClient(Path.Combine(WorkingDirectory.FullName, "Content"));
            _contentBuilderContentClient = _classicContentClient;
            _stackFrame.Push(new StackFrame(_rootCompilationOptions.GeneratedMethodName, _rootCompilationOptions.InputFile));
        }

        public RuntimeContext(
            CompilationOptions? rootCompileOptions = null,
            SubscriberContext? subscriberContext = null)
        {
            this.Random = new Random();
            _rootCompilationOptions = rootCompileOptions;
            _classicContentClient = ContentClientFactory.CreateLocalDiskContentClient(Path.Combine(WorkingDirectory.FullName, "Content"));
            _contentBuilderContentClient = _classicContentClient;
            _dataExtensionClient = DataExtensionClientFactory.CreateInMemoryDataExtensions(WorkingDirectory).Result;

            string subscriberContextFile = Path.Combine(WorkingDirectory.FullName, "subscriber.json");
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

            _stackFrame.Push(new StackFrame(_rootCompilationOptions?.GeneratedMethodName ?? "TEST", _rootCompilationOptions?.InputFile ?? null));
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
            _stackFrame.Peek().OutputStream.Append(data?.ToString());
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
            CompilerOptionsBuilder fromCodeString = new CompilerOptionsBuilder().WithSourceCode(id, code);

            return CompileAndExecuteEmbeddedCodeAsync(id, fromCodeString, code);
        }


        /// <summary>
        /// Executes embedded code - whether that came from TREATASCONTENT or CONTENT calls.
        /// </summary>
        /// <param name="id">The identifier of this particular code.  Can be the content ID, or an identifier to the TREATASCONTENT line of code</param>
        /// <param name="getCode">A delegate which produces the code.  This interface is a delegate which allows for future caching of content retrieval.</param>
        /// <returns>The result of executing the code</returns>
        internal string? CompileAndExecuteEmbeddedCodeAsync(string id, Func<FileInfo?> getCode)
        {
            FileInfo? code = getCode();
            if (code == null)
            {
                return null;
            }

            CompilerOptionsBuilder fromFileOptions = new CompilerOptionsBuilder().WithInputFile(code);

            return CompileAndExecuteEmbeddedCodeAsync(id, fromFileOptions, code);
        }


        internal string? CompileAndExecuteEmbeddedCodeAsync(string id, CompilerOptionsBuilder currentOptions, object fileInfoOrString)
        {
            CompilationOptions generatedOptions = currentOptions
                .WithOutputDirectory(_rootCompilationOptions?.OutputDirectory ?? new DirectoryInfo(Environment.CurrentDirectory))
                .WithOptimizationLevel(_rootCompilationOptions?.OptimizationLevel ?? OptimizationLevel.Debug)
                .Build();

            CompileResult compileResult = CSharpCompiler.GenerateAssemblyFromSource(generatedOptions);

            string poppedContext;

            if (fileInfoOrString is FileInfo contextFile)
            {
                PushContext(generatedOptions.GeneratedMethodName, contextFile);
            }
            else
            {
                PushContext(generatedOptions.GeneratedMethodName, fileInfoOrString.ToString());
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

        public void Dispose()
        {
            _dataExtensionClient.Dispose();
        }
    }
}
