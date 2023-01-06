// Copyright (c) 2022, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sage.Engine.Transpiler;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Runtime.Loader;
using Sage.Engine.Runtime;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// A class that compiles AMPscript source code into C#
    /// </summary>
    internal static class CSharpCompiler
    {
        public static string CompileAndExecute(CompilationOptions options)
        {
            CompileResult result = GenerateAssemblyFromSource(options);

            if (!result.EmitResult.Success)
            {
                throw new CompileCodeException(result);
            }

            var context = new RuntimeContext();
            object[] arguments = { context };
            result.Assembly
                ?.GetType("Sage.Engine.Runtime.AmpProgram")
                ?.GetMethod(options.BaseName)
                ?.Invoke(null, arguments);

            result.AssemblyContext!.Unload();

            return context.FlushOutputStream();
        }

        /// <summary>
        /// Generates a C# executable from the provided AMPscript source file that can be run with "dotnet {assembly}"
        /// </summary>
        public static CompileResult GenerateAssemblyFromSource(CompilationOptions options)
        {
            var transpiler = CSharpTranspiler.CreateFromFile(options.InputSourceFullPath);
            CompilationUnitSyntax compilationUnit = transpiler.GenerateProgram();

            // When there is whitespace in the C# code, the #line indentations don't work as expected.
            // I do not know why.
            string cSharpCode = compilationUnit.NormalizeWhitespace(indentation: string.Empty).ToString();

            (CSharpCompilation compilation, SourceText sourceText) = CompileExecutable(options, cSharpCode);
            (EmitResult emitResult, Assembly? assembly, AssemblyLoadContext? assemblyContext) = GenerateAssembly(options, compilation, sourceText);

            return new CompileResult(
                options.InputSourceFullPath,
                compilationUnit,
                compilation,
                emitResult,
                assembly,
                assemblyContext,
                cSharpCode);
        }

        /// <summary>
        /// Generates a C# executable from the provided CompilationUnit
        /// </summary>
        private static (CSharpCompilation, SourceText) CompileExecutable(CompilationOptions options, string cSharpCode)
        {
            Encoding encoding = Encoding.UTF8;
            IEnumerable<string> references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && File.Exists(a.Location))
                .Select(a => a.Location);

            IEnumerable<string> defaultNamespaces =
                new[]
                {
                    "System",
                    "System.Text",
                    "System.Collections.Generic"
                };

            // This entire round-about of going from CompilationUnitSyntax->String->SyntaxTree is to embed the source file
            // into the PDB and have it be debuggable.
            // https://stackoverflow.com/questions/50649795/how-to-debug-dll-generated-from-roslyn-compilation
            byte[] buffer = encoding.GetBytes(cSharpCode);
            var sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                new CSharpParseOptions(),
                path: options.OutputSourceFilename);

            var syntaxRootNode = (CSharpSyntaxNode)syntaxTree.GetRoot();

            SyntaxTree encoded = CSharpSyntaxTree.Create(syntaxRootNode, null, options.OutputSourceFilename, encoding);

            var compilation = CSharpCompilation.Create(
                options.OutputAssemblyFilename,
                syntaxTrees: new[] { encoded },
                references: references.Select(r => MetadataReference.CreateFromFile(r)),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithUsings(defaultNamespaces)
                    .WithOptimizationLevel(options.Optimization));

            return (compilation, sourceText);
        }

        /// <summary>
        /// Writes the in-memory representation of the compilation to an exe on disk
        /// </summary>
        private static (EmitResult, Assembly?, AssemblyLoadContext?) GenerateAssembly(CompilationOptions options, CSharpCompilation compilation, SourceText sourceText)
        {
            using var assemblyStream = new FileStream(options.OutputAssemblyFullPath, FileMode.Create);
            using var symbolsStream = new FileStream(options.OutputSymbolsFullPath, FileMode.Create);

            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb,
                pdbFilePath: options.OutputAssemblyFullPath);

            var embeddedTexts = new List<EmbeddedText>
            {
                EmbeddedText.FromSource(options.OutputSourceFilename, sourceText)
            };

            EmitResult emitResult = compilation.Emit(
                peStream: assemblyStream,
                pdbStream: symbolsStream,
                embeddedTexts: embeddedTexts,
                options: emitOptions);

            assemblyStream.Seek(0, SeekOrigin.Begin);
            symbolsStream.Seek(0, SeekOrigin.Begin);

            if (emitResult.Success)
            {
                var context = new AssemblyLoadContext(options.BaseName, true);

                Assembly assembly = context.LoadFromStream(assemblyStream, symbolsStream);

                return (emitResult, assembly, context);
            }

            return (emitResult, null, null);
        }
    }
}
