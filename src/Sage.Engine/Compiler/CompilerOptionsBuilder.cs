// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text;
using Microsoft.CodeAnalysis;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Builder for compilation options
    /// </summary>
    public class CompilerOptionsBuilder
    {
        public CompilerOptionsBuilder()
        {
            this.AssemblyStream = new MemoryStream();
            this.SymbolStream = new MemoryStream();
            this.OptimizationLevel = OptimizationLevel.Debug;
            this.OutputDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        }

        public CompilerOptionsBuilder(CompilationOptions current)
        {
            this.AssemblyStream = new MemoryStream();
            this.SymbolStream = new MemoryStream();
            this.OptimizationLevel = current.OptimizationLevel;
            this.OutputDirectory = current.OutputDirectory;
        }

        /// <summary>
        /// Path to the input ampscript file. Can be relative to the current working directory or absolute.
        /// </summary>
        public IContent Content { get; private set; }

        /// <summary>
        /// Use the provided source file to compile code. This is not compatible with WithSourceCode.
        /// </summary>
        public CompilerOptionsBuilder WithContent(IContent content)
        {
            Content = content;

            return this;
        }

        /// <summary>
        /// Optimization level of the generated code
        /// </summary>
        public OptimizationLevel OptimizationLevel { get; set; }

        /// <summary>
        /// Use the specified optimization level
        /// </summary>
        public CompilerOptionsBuilder WithOptimizationLevel(OptimizationLevel level)
        {
            this.OptimizationLevel = level;
            return this;
        }

        /// <summary>
        /// The stream to generate the PDB symbol file into
        /// </summary>
        public Stream SymbolStream { get; private set; }

        /// <summary>
        /// The stream to generate the assembly file into
        /// </summary>
        public Stream AssemblyStream { get; private set; }

        /// <summary>
        /// Use specific output streams.
        /// If not specified, in-memory streams are used by default.
        /// </summary>
        public CompilerOptionsBuilder WithOutputStreams(Stream assemblyStream, Stream pdbStream)
        {
            this.AssemblyStream = assemblyStream;
            this.SymbolStream = pdbStream;

            return this;
        }

        /// <summary>
        /// What directory to output generated files
        /// </summary>
        public DirectoryInfo OutputDirectory;

        /// <summary>
        /// Use the specified output directory for any generated artifacts
        /// </summary>
        public CompilerOptionsBuilder WithOutputDirectory(DirectoryInfo outputDirectory)
        {
            this.OutputDirectory = outputDirectory;
            return this;
        }

        public static string BuildMethodFromFilename(string filename)
        {
            var generatedMethodName = new StringBuilder();
            string generateFrom = Path.GetFileNameWithoutExtension(filename);
            for (int i = 0; i < generateFrom.Length; i++)
            {
                char thisChar = generateFrom[i];
                string ConvertInvalid(char input) => "_" + (short)input + "_";

                if (i == 0 && char.IsAsciiDigit(thisChar))
                {
                    generatedMethodName.Append(ConvertInvalid(thisChar));
                    continue;
                }

                if (char.IsAsciiDigit(thisChar) || char.IsAsciiLetter(thisChar))
                {
                    generatedMethodName.Append(thisChar);
                    continue;
                }

                generatedMethodName.Append(ConvertInvalid(thisChar));
            }

            return generatedMethodName.ToString();
        }

        /// <summary>
        /// Build the options into the <see cref="CompilationOptions"/> record
        /// </summary>
        public CompilationOptions Build()
        {
            if (this.Content == null)
            {
                throw new InvalidOperationException("No content has been specified");
            }

            string generatedMethodName = BuildMethodFromFilename(Content.Name);

            return new CompilationOptions()
            {
                Content = Content,
                GeneratedMethodName = generatedMethodName.ToString(),
                OptimizationLevel = this.OptimizationLevel,
                OutputDirectory = this.OutputDirectory,
                AssemblyStream = this.AssemblyStream,
                SymbolStream = this.SymbolStream
            };
        }
    }
}
