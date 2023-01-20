// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;
using Sage.Engine.Content;
using Sage.Engine.Data;
using Sage.Engine.Compiler;
using CompilationOptions = Sage.Engine.Compiler.CompilationOptions;

namespace Sage.Engine.Runtime
{
    /// <summary>
    /// The runtime context stores information about this rendering request
    /// </summary>
    /// <remarks>
    /// Things that belong here are variables, details about the subscriber being rendered
    /// or details about the HTTP request if this were in a cloud page context.
    /// </remarks>
    public partial class RuntimeContext : IDisposable
    {
        private readonly Stack<StackFrame> _stackFrame = new();

        private readonly Dictionary<string, SageVariable> _variables = new();
        private CompilationOptions? _rootCompilationOptions;
        private IDataExtensionClient _dataExtensionClient;
        private IContentClient _classicContentClient;
        private IContentClient _contentBuilderContentClient;
        private SubscriberContext _subscriberContext;

        public RuntimeContext(
            CompilationOptions rootCompilationOptions,
            IDataExtensionClient dataExtensionClient,
            SubscriberContext? subscriberContext)
        {
            _dataExtensionClient = dataExtensionClient;
            _classicContentClient = ContentClientFactory.CreateLocalDiskContentClient("Content");
            _contentBuilderContentClient = _classicContentClient;
            _subscriberContext = subscriberContext ?? new SubscriberContext(null);
            _rootCompilationOptions = rootCompilationOptions;
            _stackFrame.Push(new StackFrame(_rootCompilationOptions.GeneratedMethodName));
        }

        public RuntimeContext(
            CompilationOptions? rootCompileOptions = null)
        {
            this.Random = new Random();
            _dataExtensionClient = DataExtensionClientFactory.CreateInMemoryDataExtensions().Result;
            _classicContentClient = ContentClientFactory.CreateLocalDiskContentClient(Path.Combine(Environment.CurrentDirectory, "Content"));
            _contentBuilderContentClient = _classicContentClient;

            if (Path.Exists("subscriber.json"))
            {
                _subscriberContext = new SubscriberContext(File.ReadAllText("subscriber.json"));
            }
            else
            {
                _subscriberContext = new SubscriberContext(null);
            }

            _rootCompilationOptions = rootCompileOptions;
            _stackFrame.Push(new StackFrame(_rootCompilationOptions?.GeneratedMethodName ?? "TEST"));
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

        public void PushContext(string name)
        {
            _stackFrame.Push(new StackFrame(name));
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

        /// <summary>
        /// Executes embedded code - whether that came from TREATASCONTENT or CONTENT calls.
        /// </summary>
        /// <param name="id">The identifier of this particular code.  Can be the content ID, or an identifier to the TREATASCONTENT line of code</param>
        /// <param name="getCode">A delegate which produces the code.  This interface is a delegate which allows for future caching of content retrieval.</param>
        /// <returns>The result of executing the code</returns>
        internal async Task<string?> CompileAndExecuteEmbeddedCodeAsync(string id, Func<Task<FileInfo?>> getCode)
        {
            FileInfo? code = await getCode();
            if (code == null)
            {
                return null;
            }

            CompilationOptions generatedOptions = new CompilerOptionsBuilder()
                .WithInputFile(code)
                .WithOutputDirectory(_rootCompilationOptions?.OutputDirectory ?? new DirectoryInfo(Environment.CurrentDirectory))
                .WithOptimizationLevel(_rootCompilationOptions?.OptimizationLevel ?? OptimizationLevel.Debug)
                .Build();

            CompileResult compileResult = CSharpCompiler.GenerateAssemblyFromSource(generatedOptions);

            PushContext(generatedOptions.GeneratedMethodName);
            compileResult.Execute(this);
            return PopContext();
        }

        public void Dispose()
        {
            _dataExtensionClient.Dispose();
        }
    }
}
