namespace rune.etc
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text;
    using ancient.runtime;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class CSharpCompile
    {
        public static Assembly Build(string id, string code)
        {
            var dd = typeof(Enumerable).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(dd);

            var refs = coreDir.GetFiles("*.dll").Select(x => MetadataReference.CreateFromFile(x.FullName)).ToList();

            refs.AddRange(new []
            {
                MetadataReference.CreateFromFile(typeof(ldx).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IDevice).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location)
            });

            var compilation = CSharpCompilation.Create($"{id}.image")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true))
                .AddReferences(refs)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

            using var ms = new MemoryStream();		
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                var b = new StringBuilder();
                foreach (var diagnostic in result.Diagnostics)
                    b.AppendLine(diagnostic.ToString());
                throw new Exception(b.ToString());
            }
            ms.Seek(0, SeekOrigin.Begin);
            return AssemblyLoadContext.Default.LoadFromStream(ms);
        }
    }
}