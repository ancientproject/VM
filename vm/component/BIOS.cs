namespace vm.component
{
    using System;
    using flame.runtime;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class BIOS
    {
        private const string ESC = "\x1b";

        public static IReadOnlyCollection<uint> GetILCode()
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

            list.Add(0xB00B5000);
            return list.AsReadOnly();
        }

        private static Instruction[] WriteLine(string line, Color color)
        {
            line = $"{line}{Environment.NewLine}";
            var result = $"{GetColorForegroundString(color)}{line}{GetColorForegroundString(Color.White)}";

            return result.Select(x => new push_a(0x2, 0x5, (short) x)).Cast<Instruction>().ToArray();
        }

        private static string GetColorForegroundString(Color c) => 
            string.Concat(ESC, "[38;2;", c.R.ToString(), ";", c.G.ToString(), ";", c.B.ToString(), "m");
    }
}