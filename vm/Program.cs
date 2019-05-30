namespace vm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using component;
    using dev;
    using dev.Internal;
    using models;
    using models.list;
    using MoreLinq;
    using static System.Console;
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Title = "cpu_host";

            IntToCharConverter.Register<char>();

            var bus = new Bus();

            bus.Add(new Terminal(0x1));

            var core = bus.Cpu;

            //core.State.Load(new []
            //{
            //    0x1064, // set &r1
            //    0x11C8, // set &r2
            //    0x2201, // sum &r2 &r3 -> r1
            //    0xA001, // print &r1
            //    0xA002, // print &r2
            //    0x3120, // swipe &r1 &r2
            //    0xA001, // print &r1
            //    0xA002, // print &r2
            //    0xDEAD  // halt
            //});
            //core.State.Load(new uint[]
            //{
            // // 0x rrruux
            // // 0x 123121
            //    0xABCDEFE, // warm up
            //    0x1000600, // set &r1
            //    0xF000003, // dump last reg
            //    0x1100C00, // set &r2
            //    0xF000003, // dump last reg
            //    0x2021000, // sum &r2 &r3 -> r1
            //    0xF000003, // dump last reg
            //    0xE000103, // print &r1
            //    0xE000203, // print &r2
            //    0x3120000, // swipe &r1 &r2
            //    0xE000103, // print &r1
            //    0xE000203, // print &r2
            //    //core.Compile(0xF, 0x0, 0x0, 0xE, ((byte)'H' & 0xF0) >> 4, ((byte)'H' & 0xF) >> 0, 0xC),
            //    0x8F000C0, // ref point to *
            //    0xF00E48C, // push 'H'
            //    0xF00E45C, // push 'E'
            //    0xF00E4CC, // push 'L'
            //    0xF00E4CC, // push 'L'
            //    0xF00E4FC, // push 'O'
            //    0xF00E20C, // push ' '
            //    0xF00E57C, // push 'W'
            //    0xF00E4FC, // push 'O'
            //    0xF00E52C, // push 'R'
            //    0xF00E4CC, // push 'L'
            //    0xF00E44C, // push 'D'
            //    0xF00E21C, // push '!'
            //    0xF00E0AC, // push '\n'
            //    0xF00E00F, // pop, and merge
            //    0x8F000F0, // jump to *
            //    //core.Compile(0xF, 0x0, 0x0, 0xE, 0x0, 0x0, 0xF),
            //    0xDEAD     // halt
            //});
            var mem = new List<object>();

            //mem.AddRange(new object[]
            //{
            //    new warm(), 

            //    new loadi(0x0, 153), 
            //    "x: ".Cast_f<push_a>(0x1, 0x6),
            //    new push_x_debug(0x1, 0x6, 0x0),
            //    new push_a(0x1, 0x6, (byte)'\n'),

            //    new loadi(0x1, 42),
            //    "y: ".Cast_f<push_a>(0x1, 0x6),
            //    new push_x_debug(0x1, 0x6, 0x1),
            //    new push_a(0x1, 0x6, (byte)'\n'),

            //    new sum(0x2, 0x0, 0x1),

            //    "x + y == ".Cast_f<push_a>(0x1, 0x6),
            //    new push_x_debug(0x1, 0x6, 0x2),


            //    new push_a(0x1, 0x6, (byte)'\n'),
            //    new push_a(0x1, 0x7, 0), // in dev pop and merge
            //    new halt()
            //});

            mem.AddRange(new object[]
            {
                new warm(),
                new ref_t(0xA),                                       // .label 0xA
                "svack pidor ".Cast_f<push_a>(0x1, 0x6),    // push_a 'svack pidor ', 0
                new push_a(0x1, 0x7, 0),          // pop, merge
                new push_a(0x1, 0x3, 0),          // clear
                new jump_t(0xA),                                      // jump_to_label 0xA
                new halt()
            });

            var list = new List<Instruction>();
            foreach (var o in mem)
            {
                if(o.GetType().IsArray)
                    list.AddRange(((Instruction[])o));
                else
                    list.Add((Instruction)o);
            }

            core.State.Load(list.Select(x => (uint)x).ToArray());
            //core.State.Load(new uint[]
            //{
            //    0xABCDEFE,
            //    0xF16E48C,
            //    0xF16E4FC,
            //    0xF16E21C,
            //    0xF16E0AC,
            //    0xF17E00F,
            //    0xDEAD
            //});

            while (core.State.halt == 0)
            {
                await core.Step();
                await Task.Delay(10);
            }

            ReadKey();
        }
    }

   
    
}