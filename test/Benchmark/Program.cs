namespace Benchmark
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using flame.compiler.tokens;
    using flame.runtime;
    using Sprache;
    using vm.component;
    using vm.dev;
    using vm.dev.Internal;

    [InProcess]
    [RPlotExporter, RankColumn]
    public class JobCompiling
    {
        private Bus bus;
        private uint push_a_to_null;
        private uint push_a_to_rel;
        private uint loadi;

        [Benchmark(Description = ".loadi")]
        public async Task ExecuteLoadIDefault()
        {
            await bus.Cpu.Step(loadi);
        }
        [Benchmark(Description = ".push_a <null-dev>")]
        public async Task ExecutePushANullDev()
        {
            await bus.Cpu.Step(push_a_to_null);
        }
        [Benchmark(Description = ".push_a <rel-dev>")]
        public async Task ExecutePushARelDev()
        {
            await bus.Cpu.Step(push_a_to_rel);
        }

        [GlobalSetup]
        public void Setup()
        {
            IntToCharConverter.Register<char>();
            bus = new Bus();
            bus.Add(new NullDevice());
            bus.Add(new RelDevice());
            push_a_to_rel = (uint)new push_a(0x4, 0x6, 'x').Assembly();
            push_a_to_null = (uint)new push_a(0x5, 0x6, 'x').Assembly();
            loadi = (uint)new loadi(0x5, 0x6).Assembly();
        }
    }
    public class RelDevice : AbstractDevice
    {
        public RelDevice() : base(0x4, "<null-device>") { }

        private StringBuilder relMemory = new StringBuilder();

        [ActionAddress(0x6)]
        public void StageChar(char c)
        {
            if (c == 0xA)
                relMemory.Append(Environment.NewLine);
            else 
                relMemory.Append(c);

            if (relMemory.Length > uint.MaxValue / 10)
                relMemory.Clear();
        }
    }
    public class NullDevice : AbstractDevice
    {
        public NullDevice() : base(0x5, "<null-device>") { }

        [ActionAddress(0x6)]
        public void StageChar(char c)
        {
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<JobCompiling>();
            Console.WriteLine(summary);
        }
    }
}
