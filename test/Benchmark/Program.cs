namespace Benchmark
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using flame.runtime;
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
        public async Task S1() => await bus.cpu.Step(loadi);

        [Benchmark(Description = ".push_a <null-dev> fast-off")]
        public async Task S2() => await bus.cpu.Step(push_a_to_null);

        [Benchmark(Description = ".push_a <rel-dev> fast-off")]
        public async Task S3() => await bus.cpu.Step(push_a_to_rel);

        [Benchmark(Description = ".push_a <null-dev> fast-on")]
        public async Task S4() => await bus.cpu.Step(push_a_to_null);

        [Benchmark(Description = ".push_a <rel-dev> fast-on")]
        public async Task S5() => await bus.cpu.Step(push_a_to_rel);

        [GlobalSetup(Targets = new[] {nameof(S5), nameof(S4)})]
        public void EnableFastWrite()
        {
            MemoryManagement.FastWrite = true;
            Setup();
        }

        [GlobalSetup(Targets = new[] {nameof(S1), nameof(S2), nameof(S3)})]
        public void DisableFastWrite()
        {
            MemoryManagement.FastWrite = false;
            Setup();
        }

        private void Setup()
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

        private readonly StringBuilder relMemory = new StringBuilder();

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
