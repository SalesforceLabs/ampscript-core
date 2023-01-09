// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic.CompilerServices;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Options specifying how the AMPscript code compiler should operate
    /// </summary>
    /// <param name="InputName">Name of the input, usually the file name of the file</param>
    /// <param name="InputFile">Path to the input ampscript file. Can be relative to the current working directory or absolute.</param>
    /// <param name="SourceCode">The AMPscript source code</param>
    /// <param name="OptimizationLevel">Optimization level of the generated code</param>
    /// <param name="OutputDirectory">What directory to output generated files</param>
    /// <param name="AssemblyStream">Stream for the assembly to be generated into</param>
    /// <param name="SymbolStream">Stream for the symbol file to be generated into</param>
    public record CompilationOptions(
        string InputName,
        FileInfo InputFile,
        string SourceCode,
        OptimizationLevel OptimizationLevel,
        DirectoryInfo OutputDirectory,
        Stream AssemblyStream,
        Stream SymbolStream)
    {
        private static Regex AlphaNumeric = new Regex("[^a-zA-Z0-9]");

        /// <summary>
        /// Generates a method name from the provided inputs
        /// </summary>
        public string GeneratedMethodName
        {
            get
            {
                return CompilationOptions.AlphaNumeric.Replace(this.InputName, "_");
            }
        }

        /// <summary>
        /// Pre-defined generated assembly file
        /// </summary>
        public FileInfo GeneratedAssemblyOutput
        {
            get
            {
                return new FileInfo(Path.Combine(OutputDirectory.FullName, $"{GeneratedMethodName}.dll"));
            }
        }
    }
}
