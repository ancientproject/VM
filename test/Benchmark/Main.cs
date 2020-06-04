namespace Benchmark
{
    using System;
    using System.Linq;
    using BenchmarkDotNet.Running;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine(BenchmarkRunner.Run<JobDeconstruct>());
                return;
            }

            switch (args.First())
            {
                //case "compiling":
                //    Console.WriteLine(BenchmarkRunner.Run<JobCompiling>()); return;
                //case "interned": 
                //    Console.WriteLine(BenchmarkRunner.Run<JobInternString>()); return;
            }

            
        }
    }
}