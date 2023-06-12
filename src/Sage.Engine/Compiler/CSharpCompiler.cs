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
using Basic.Reference.Assemblies;
using Sage.Engine.Data;

namespace Sage.Engine.Compiler
{
    /// <summary>
    /// A class that compiles AMPscript source code into C# and optionally will execute it.
    /// </summary>
    internal static class CSharpCompiler
    {
        public static string CompileAndExecute(CompilationOptions options, RuntimeContext context)
        {
            if (string.IsNullOrEmpty(options.SourceCode))
            {
                options.SourceCode = File.ReadAllText(options.InputFile.FullName);
            }
            CompileResult result = GenerateAssemblyFromSource(options);

            result.Execute(context);

            result.AssemblyContext!.Unload();
            options.AssemblyStream.Close();
            options.SymbolStream.Close();

            // This pops the original and only context.
            return context.PopContext();
        }

        /// <summary>
        /// Generates C# code from the provided AMPscript
        /// </summary>
        public static CompileResult GenerateAssemblyFromSource(CompilationOptions options)
        {
            if (options.SourceCode == null)
            {
                throw new ArgumentNullException(nameof(options.SourceCode));
            }

            CSharpTranspiler transpiler = CSharpTranspiler.CreateFromSource(options.InputFile.FullName, options.GeneratedMethodName, options.SourceCode);

            CompilationUnitSyntax compilationUnit = transpiler.GenerateCode();

            // When there is whitespace in the C# code, the #line indentations don't work as expected.
            // I do not know why.
            string cSharpCode = compilationUnit.NormalizeWhitespace(indentation: string.Empty).ToString();

            (CSharpCompilation compilation, SourceText sourceText) = GenerateCompilation(options, cSharpCode);
            (EmitResult emitResult, Assembly? assembly, AssemblyLoadContext? assemblyContext) = GenerateAssembly(options, compilation, sourceText);

            return new CompileResult(
                options.InputFile.FullName,
                options.GeneratedMethodName,
                compilationUnit,
                compilation,
                emitResult,
                assembly,
                assemblyContext,
                cSharpCode);
        }

        /// <summary>
        /// Generates a CSharpCompilation from the provided source
        /// </summary>
        private static (CSharpCompilation, SourceText) GenerateCompilation(CompilationOptions options, string cSharpCode)
        {
            Encoding encoding = Encoding.UTF8;

            // Basic.Reference.Assemblies.Net70.References.All
            var metadataReference = Net70.References.All.ToList();
            metadataReference.Add(MetadataReference.CreateFromFile(typeof(RuntimeContext).Assembly.Location));
            metadataReference.Add(MetadataReference.CreateFromFile(typeof(IDataExtensionClient).Assembly.Location));

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
                new CSharpParseOptions());

            var syntaxRootNode = (CSharpSyntaxNode)syntaxTree.GetRoot();

            SyntaxTree encoded = CSharpSyntaxTree.Create(
                syntaxRootNode,
                path: $"{options.InputName}.generated.cs",
                encoding: encoding);

            var compilation = CSharpCompilation.Create(
                options.GeneratedAssemblyOutput.Name,
                syntaxTrees: new[] { encoded },
                references: metadataReference,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithUsings(defaultNamespaces)
                    .WithOptimizationLevel(options.OptimizationLevel));

            return (compilation, sourceText);
        }

        /// <summary>
        /// Writes the in-memory representation of the compilation to an exe on disk
        /// </summary>
        private static (EmitResult, Assembly?, AssemblyLoadContext?) GenerateAssembly(CompilationOptions options, CSharpCompilation compilation, SourceText sourceText)
        {
            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb);

            var embeddedTexts = new List<EmbeddedText>
            {
                EmbeddedText.FromSource($"{options.InputName}.generated.cs", sourceText)
            };

            EmitResult emitResult = compilation.Emit(
                peStream: options.AssemblyStream,
                pdbStream: options.SymbolStream,
                embeddedTexts: embeddedTexts,
                options: emitOptions);

            options.AssemblyStream.Seek(0, SeekOrigin.Begin);
            options.SymbolStream.Seek(0, SeekOrigin.Begin);

            if (emitResult.Success)
            {
                var context = new AssemblyLoadContext(options.InputName, true);

                Assembly assembly = context.LoadFromStream(options.AssemblyStream, options.SymbolStream);

                return (emitResult, assembly, context);
            }

            return (emitResult, null, null);
        }
    }
}
