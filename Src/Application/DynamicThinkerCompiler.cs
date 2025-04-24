using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.IO;
using System.Reflection;

using Domain.Interfaces;
using Domain.Entities.Players;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;


namespace Application
{
    public class DynamicThinkerCompiler
    {

        public IThinker LoadThinkerFromFile(string filePath)
        { return ParseInstance(CompileAssembly(filePath)); }

        private Assembly CompileAssembly(string filePath)
        {
            string code = File.ReadAllText(filePath);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            string assemblyName = Path.GetRandomFileName();
            var references = new List<MetadataReference>
            {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IThinker).Assembly.Location)
            };


            // Add all currently loaded assemblies to the references
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }


            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    string errors = string.Join(Environment.NewLine, failures.Select(diagnostic => $"{diagnostic.Id}: {diagnostic.GetMessage()}"));
                    throw new InvalidOperationException($"Compilation errors in file {filePath}:{Environment.NewLine}{errors}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }

        private IThinker ParseInstance(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IThinker).IsAssignableFrom(type) && !type.IsInterface)
                {
                    return (IThinker)Activator.CreateInstance(type);
                }
            }

            throw new InvalidOperationException("No IThinker instances found in file");
        }
    }
}
