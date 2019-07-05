namespace Benchmark
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using ancient.runtime;
    using ancient.runtime.hardware;
    using vm.component;

    [InProcess]
    [RPlotExporter, RankColumn]
    public class JobCompiling
    {
        private Bus bus;
        private uint push_a_to_null;
        private uint push_a_to_rel;
        private uint loadi;

        [Benchmark(Description = ".ldi")]
        public void S1() => bus.cpu.Step(loadi);

        [Benchmark(Description = ".mva <null-dev> fast-off")]
        public void S2() => bus.cpu.Step(push_a_to_null);

        [Benchmark(Description = ".mva <rel-dev> fast-off")]
        public void S3() => bus.cpu.Step(push_a_to_rel);

        [Benchmark(Description = ".mva <null-dev> fast-on")]
        public void S4() => bus.cpu.Step(push_a_to_null);

        [Benchmark(Description = ".mva <rel-dev> fast-on")]
        public void S5() => bus.cpu.Step(push_a_to_rel);

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
            push_a_to_rel = (uint)new mva(0x4, 0x6, 'x').Assembly();
            push_a_to_null = (uint)new mva(0x5, 0x6, 'x').Assembly();
            loadi = (uint)new ldi(0x5, 0x6).Assembly();
        }
    }
    public class RelDevice : Device
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
    public class NullDevice : Device
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
