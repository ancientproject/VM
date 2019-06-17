namespace vm.component
{
    using System;
    using flame.runtime;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class BIOS
    {
        public static IReadOnlyCollection<uint> GetILCode()
        {
            /* @0x11 */
            var tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";
            /* @0x12 */
            var ec = Environment.GetEnvironmentVariable("FLAME_ERROR") != "0";
            /* @0x13 */
            var km = Environment.GetEnvironmentVariable("FLAME_KEEP_MEMORY") == "1";
            /* @0x14 */
            var fw = Environment.GetEnvironmentVariable("FLAME_MEM_FAST_WRITE") == "1";

            var list = new List<uint>
            {
                /* init memory */
                new loadi_x(0xFF, 0x20, 0xF),
                /* set flagms */
                new loadi_x(0x11, tc),
                new loadi_x(0x12, ec),
                new loadi_x(0x13, km),
                new loadi_x(0x14, fw),
            };
            return list.AsReadOnly();
        }


        private const string ESC = "\x1b";

        private static IReadOnlyCollection<uint> _GetILCode()
        {
            var list = new List<uint>();

            list.Add(new warm());

            list.AddRange(WriteLine("⠜ ⠗ ⠅ ⠷ ⠷ ⠾ ⠾ ⠵ ⠸ ⠅ ⠚ ⠜ ⠵ ⠇", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("-    ------   ----  --- ------", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("00   PWR-BS   BASE  [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("01   BIOS-X   BASE  [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("02   SYS-CE   BASE  [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("-    ------   ----  --- ------", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("⠚ ⠷ ⠸ ⠵ ⠇ ⠅ ⠗ ⠾ ⠾ ⠅ ⠜ ⠜ ⠷ ⠙", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("kern.mod.cmn ....... [3 PASSED]", Color.LawnGreen).Select(x => (uint)x));

            list.AddRange(WriteLine("booting up...", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("Flame BIOS // VER_1.2 :: DEFAULT", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("Mount devices...", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("⠵ ⠙ ⠚ ⠷ ⠜ ⠷ ⠗ ⠅ ⠅ ⠾ ⠵ ⠸ ⠇ ⠾ ⠜", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("⠵ ⠷ ⠜ ⠇ ⠜ ⠙ ⠷ ⠅ ⠚ ⠾ ⠸ ⠅ ⠵ ⠾ ⠗", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("-    ------   ------   --- ------", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("00   SYS-ID   LFS-CK   [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("01   SYS-MC   UFS-CK   [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("02   ABL-SS   SUB-CK   [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("03   RUN-XX   RUN-CK   [+] [PASS]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("-    ------   ------   --- ------", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("vis_sys.mod.cmn ...... [4 PASSED]", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("⠇ ⠗ ⠙ ⠚ ⠜ ⠾ ⠅ ⠅ ⠵ ⠷ ⠸ ⠵ ⠷ ⠜ ⠾", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("⠜ ⠷ ⠷ ⠾ ⠚ ⠜ ⠾ ⠸ ⠵ ⠅ ⠙ ⠅ ⠇ ⠵ ⠗", Color.LawnGreen).Select(x => (uint)x));
            list.AddRange(WriteLine("Execute main code...", Color.LawnGreen).Select(x => (uint)x));

            return list.AsReadOnly();
        }

        private static Instruction[] WriteLine(string line, Color color)
        {
            line = $"{line}{Environment.NewLine}";
            var result = $"{GetColorForegroundString(color)}{line}{GetColorForegroundString(Color.White)}";

            return result.Select(x => new push_a(0x2, 0x5, x)).Cast<Instruction>().ToArray();
        }

        private static string GetColorForegroundString(Color c) => 
            string.Concat(ESC, "[38;2;", c.R.ToString(), ";", c.G.ToString(), ";", c.B.ToString(), "m");
    }
}