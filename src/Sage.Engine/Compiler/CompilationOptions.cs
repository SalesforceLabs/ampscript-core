// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using Microsoft.CodeAnalysis;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// Options specifying how the AMPscript code compiler should operate
    /// </summary>
    /// <param name="Content">The AMPscript source code</param>
    /// <param name="GeneratedMethodName">The name of the generated method in the assembly</param>
    /// <param name="OptimizationLevel">Optimization level of the generated code</param>
    /// <param name="OutputDirectory">What directory to output generated files</param>
    /// <param name="AssemblyStream">Stream for the assembly to be generated into</param>
    /// <param name="SymbolStream">Stream for the symbol file to be generated into</param>
    public class CompilationOptions
    {
        public required IContent Content { get; set; }
        public required string GeneratedMethodName { get; set; }
        public required OptimizationLevel OptimizationLevel { get; set; }
        public required DirectoryInfo OutputDirectory { get; set; }
        public required Stream AssemblyStream { get; set; }
        public required Stream SymbolStream { get; set; }

        public CompilationOptions Clone()
        {
            var newOptions = (CompilationOptions)this.MemberwiseClone();
            newOptions.AssemblyStream = new MemoryStream();
            newOptions.SymbolStream = new MemoryStream();
            return newOptions;
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
