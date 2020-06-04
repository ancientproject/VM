namespace Benchmark
{
    using ancient.runtime;
    using ancient.runtime.tools;
    using BenchmarkDotNet.Attributes;

    [InProcess]
    [RPlotExporter, RankColumn]
    public class JobDeconstruct
    {
        public static readonly Unicast<ushort, ulong> u16 = new Unicast<ushort, ulong>();

        [Benchmark(Description = "deconstruct (old) [mask, bit array, take-while, alloc, u16 unicast]")]
        public void Dec1()
        {
            BitwiseContainer container = 0xABCDEF12345678;
            var pfx = u16 & ((container & 0x00F0000000000000));
            var iid = u16 & ((container & 0x000F000000000000));
            var r1  = u16 & ((container & 0x0000F00000000000));
            var r2  = u16 & ((container & 0x00000F0000000000));
            var r3  = u16 & ((container & 0x000000F000000000));
            var u1  = u16 & ((container & 0x0000000F00000000));
            var u2  = u16 & ((container & 0x00000000F0000000));
            var x1  = u16 & ((container & 0x000000000F000000));
            var x2  = u16 & ((container & 0x0000000000F00000));
            var x3  = u16 & ((container & 0x00000000000F0000));
            var x4  = u16 & ((container & 0x000000000000F000));
            var o1  = u16 & ((container & 0x0000000000000F00));
            var o2  = u16 & ((container & 0x00000000000000F0));
            var o3  = u16 & ((container & 0x000000000000000F));
            iid = u16 & ((pfx << 4) | iid );
        }
        [Benchmark(Description = "deconstruct (new) [mask, shift container, u16 unicast]")]
        public void Dec2()
        {
            var shifter = ShiftFactory.CreateByIndex(52);

            ulong container = 0xABCDEF12345678;
            var pfx = u16 & ((container & 0x00F0000000000000) >> shifter.Shift());
            var iid = u16 & ((container & 0x000F000000000000) >> shifter.Shift());
            var r1  = u16 & ((container & 0x0000F00000000000) >> shifter.Shift());
            var r2  = u16 & ((container & 0x00000F0000000000) >> shifter.Shift());
            var r3  = u16 & ((container & 0x000000F000000000) >> shifter.Shift());
            var u1  = u16 & ((container & 0x0000000F00000000) >> shifter.Shift());
            var u2  = u16 & ((container & 0x00000000F0000000) >> shifter.Shift());
            var x1  = u16 & ((container & 0x000000000F000000) >> shifter.Shift());
            var x2  = u16 & ((container & 0x0000000000F00000) >> shifter.Shift());
            var x3  = u16 & ((container & 0x00000000000F0000) >> shifter.Shift());
            var x4  = u16 & ((container & 0x000000000000F000) >> shifter.Shift());
            var o1  = u16 & ((container & 0x0000000000000F00) >> shifter.Shift());
            var o2  = u16 & ((container & 0x00000000000000F0) >> shifter.Shift());
            var o3  = u16 & ((container & 0x000000000000000F) >> shifter.Shift());
            iid = u16 & ((pfx << 4) | iid );
        }
        [Benchmark(Description = "deconstruct (new) [mask, u16 unicast]")]
        public void Dec3()
        {
            ulong container = 0xABCDEF12345678;
            var pfx = u16 & ((container & 0x00F0000000000000) >> 52);
            var iid = u16 & ((container & 0x000F000000000000) >> 48);
            var r1  = u16 & ((container & 0x0000F00000000000) >> 44);
            var r2  = u16 & ((container & 0x00000F0000000000) >> 40);
            var r3  = u16 & ((container & 0x000000F000000000) >> 36);
            var u1  = u16 & ((container & 0x0000000F00000000) >> 32);
            var u2  = u16 & ((container & 0x00000000F0000000) >> 28);
            var x1  = u16 & ((container & 0x000000000F000000) >> 24);
            var x2  = u16 & ((container & 0x0000000000F00000) >> 20);
            var x3  = u16 & ((container & 0x00000000000F0000) >> 16);
            var x4  = u16 & ((container & 0x000000000000F000) >> 12);
            var o1  = u16 & ((container & 0x0000000000000F00) >> 8);
            var o2  = u16 & ((container & 0x00000000000000F0) >> 4);
            var o3  = u16 & ((container & 0x000000000000000F) >> 0);
            iid = u16 & ((pfx << 4) | iid );
        }
        [Benchmark(Description = "deconstruct (new) [mask, without u16 unicast]")]
        public void Dec4()
        {
            ulong container = 0xABCDEF12345678;
            var pfx = ((container & 0x00F0000000000000) >> 52);
            var iid = ((container & 0x000F000000000000) >> 48);
            var r1  = ((container & 0x0000F00000000000) >> 44);
            var r2  = ((container & 0x00000F0000000000) >> 40);
            var r3  = ((container & 0x000000F000000000) >> 36);
            var u1  = ((container & 0x0000000F00000000) >> 32);
            var u2  = ((container & 0x00000000F0000000) >> 28);
            var x1  = ((container & 0x000000000F000000) >> 24);
            var x2  = ((container & 0x0000000000F00000) >> 20);
            var x3  = ((container & 0x00000000000F0000) >> 16);
            var x4  = ((container & 0x000000000000F000) >> 12);
            var o1  = ((container & 0x0000000000000F00) >> 8);
            var o2  = ((container & 0x00000000000000F0) >> 4);
            var o3  = ((container & 0x000000000000000F) >> 0);
            iid = ((pfx << 4) | iid );
        }
        [Benchmark(Description = "deconstruct (new) [mask, classic u16 cast]")]
        public void Dec5()
        {
            ulong container = 0xABCDEF12345678;
            var pfx = (ushort)((container & 0x00F0000000000000) >> 52);
            var iid = (ushort)((container & 0x000F000000000000) >> 48);
            var r1  = (ushort)((container & 0x0000F00000000000) >> 44);
            var r2  = (ushort)((container & 0x00000F0000000000) >> 40);
            var r3  = (ushort)((container & 0x000000F000000000) >> 36);
            var u1  = (ushort)((container & 0x0000000F00000000) >> 32);
            var u2  = (ushort)((container & 0x00000000F0000000) >> 28);
            var x1  = (ushort)((container & 0x000000000F000000) >> 24);
            var x2  = (ushort)((container & 0x0000000000F00000) >> 20);
            var x3  = (ushort)((container & 0x00000000000F0000) >> 16);
            var x4  = (ushort)((container & 0x000000000000F000) >> 12);
            var o1  = (ushort)((container & 0x0000000000000F00) >> 8);
            var o2  = (ushort)((container & 0x00000000000000F0) >> 4);
            var o3  = (ushort)((container & 0x000000000000000F) >> 0);
            iid = (ushort)((pfx << 4) | iid );
        }
    }
}