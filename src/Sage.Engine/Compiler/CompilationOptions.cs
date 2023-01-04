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
    /// <param name="InputSourceFile">Path to the input ampscript file. Can be relative to the current working directory or absolute.</param>
    /// <param name="OutputPath">Output path to place the generated assemblies & artifacts. Can be relative or absolute.</param>
    /// <param name="Optimization">Optimization level to compile code. Code compiled for release cannot be debugged.</param>
    internal record CompilationOptions(
        string InputSourceFile,
        string OutputPath,
        OptimizationLevel Optimization)
    {
        /// <summary>
        /// The name to base the .cs, .dll, .pdb files from.
        /// </summary>
        /// <example>foo</example>
        public string BaseName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(InputSourceFile);
            }
        }

        /// <summary>
        /// The full absolute path to the input source file
        /// </summary>
        /// <example>c:\bar\foo.ampscript</example>
        public string InputSourceFullPath
        {
            get
            {
                return Path.GetFullPath(InputSourceFile);
            }
        }

        /// <summary>
        /// The filename of the output assembly
        /// </summary>
        /// <example>foo.dll</example>
        public string OutputAssemblyFilename
        {
            get
            {
                return $"{BaseName}.dll";
            }
        }

        /// <summary>
        /// The full path to the output assembly
        /// </summary>
        /// <example>c:\bar\foo.dll</example>
        public string OutputAssemblyFullPath
        {
            get
            {
                return Path.Combine(OutputPath, OutputAssemblyFilename);
            }
        }

        /// <summary>
        /// The filename for the generated source file
        /// </summary>
        /// <example>foo.cs</example>
        public string OutputSourceFilename
        {
            get
            {
                return $"{BaseName}.cs";
            }
        }

        /// <summary>
        /// The filename for the generated PDB file
        /// </summary>
        /// <example>foo.pdb</example>
        public string OutputSymbolsFilename
        {
            get
            {
                return $"{BaseName}.pdb";
            }
        }

        /// <summary>
        /// The full path to the symbols file
        /// </summary>
        /// <example>c:\bar\foo.pdb</example>
        public string OutputSymbolsFullPath
        {
            get
            {
                return Path.Combine(OutputPath, OutputSymbolsFilename);
            }
        }
    }
}
