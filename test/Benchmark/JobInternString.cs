namespace Benchmark
{
    using System.Collections.Generic;
    using ancient.runtime.@unsafe;
    using BenchmarkDotNet.Attributes;
    public static class StringLiteralMapClassic
    {
        public static string GetInternedString(string str, bool AddIfNotFound)
        {
            if (literalStorage.Contains(str))
                return str;
            if (AddIfNotFound)
            {
                literalStorage.Add(str);
                return str;
            }
            return null;
        }

        private static readonly HashSet<string> literalStorage = new HashSet<string>();

        public static void Clear() => literalStorage.Clear();
    }
    [InProcess]
    [RPlotExporter, RankColumn]
    public class JobInternString
    {
        private static NativeString native;

        [Benchmark(Description = "intern string [managed non-insert]")]
        public void NonNativeFalseFound()
        {
            StringLiteralMapClassic.GetInternedString("test_string", false);
        }
        [Benchmark(Description = "intern string [native non-insert]")]
        public unsafe void NativeFalseFound()
        {
            fixed(NativeString* p = &native)
                StringLiteralMap.GetInternedString(p, false);
        }
        [GlobalSetup]
        public static void Asd()
        {
            native = NativeString.Wrap("test_string");
        }
        [Benchmark(Description = "intern string [native insert, auto free]")]
        public unsafe void AddNative()
        {
            fixed(NativeString* p = &native)
                StringLiteralMap.GetInternedString(p, true);
            StringLiteralMap.Clear();
        }

        [Benchmark(Description = "intern string [managed insert, auto free]")]
        public void AddClassic()
        {
            StringLiteralMapClassic.GetInternedString("test_string", true);
            StringLiteralMapClassic.Clear();
        }
    }
}