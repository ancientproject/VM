namespace rune.etc
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ancient.runtime;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class CSharpCompile
    {
        public static byte[] Build(string id, string code)
        {
            Console.Write($"{":thought_balloon:".Emoji()} Mount '{id}'...".Color(Color.DimGray));
            var dd = typeof(Enumerable).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(dd);
            var refs = new List<MetadataReference>();

            refs.AddRange(new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),

                MetadataReference.CreateFromFile($"{Path.Combine(coreDir.FullName, "netstandard.dll")}"),
                MetadataReference.CreateFromFile($"{Path.Combine(coreDir.FullName, "System.Runtime.dll")}"),
                MetadataReference.CreateFromFile($"{Path.Combine(coreDir.FullName, "System.Runtime.Extensions.dll")}"),
                MetadataReference.CreateFromFile($"{Path.Combine(coreDir.FullName, "Microsoft.CSharp.dll")}"),

                MetadataReference.CreateFromFile(typeof(ldx).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IDevice).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location)
            });

            var compilation = CSharpCompilation.Create($"{id}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Debug))
                .AddReferences(refs)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
            var temp = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var result = compilation.Emit(temp);
            if (result.Success)
                Console.WriteLine($".. OK".Color(Color.DimGray));
            else
                Console.WriteLine($".. FAIL".Color(Color.DimGray));

            if (result.Success)
                return File.ReadAllBytes(temp);
            foreach (var diagnostic in result.Diagnostics)
                Console.WriteLine(diagnostic.ToString().Color(Color.Red));
            return null;
        }
    }
}