namespace ancient.runtime.emit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class AssemblyTag
    {
        public static AssemblyTag Null = new AssemblyTag(SignType.UnSecurity, ArchType.Any, 2);

        public const string ElementaryFlameTag = "EF";
        public enum SignType
        {
            UnSecurity,
            Signed
        }

        public enum ArchType
        {
            x64,
            x32,
            Any = 9
        }

        public SignType Sign { get; set; }
        public ArchType Arch { get; set; }
        public byte Version { get; set; }

        public int Year
        {
            get => 2010 + (year - 'A');
            set => year = getAlphabetChars().ToArray()[value - 2010];
        }

        public int Month
        {
            get => (month - 'A');
            set => month = getAlphabetChars().ToArray()[value];
        }

        private char year { get; set; }
        private char month { get; set; }

        private IEnumerable<char> getAlphabetChars() 
            => Enumerable.Range('A', 'Z').Select(x => (char) x);

        public AssemblyTag() {}

        public AssemblyTag(SignType st, ArchType at, byte ver = 2)
        {
            Sign = st;
            Arch = at;
            Version = ver;
            Year = DateTime.UtcNow.Year;
            Month = DateTime.UtcNow.Month;
        }

        public static AssemblyTag Parse(string str)
        {
            if (!IsTag(str))
                return Null;
            var asm = new AssemblyTag();
            var stack = new Stack<char>(str.Reverse());

            stack.Pop(); // E
            stack.Pop(); // F

            asm.Version = byte.Parse($"{stack.Pop()}{stack.Pop()}");
            asm.Sign = (SignType) byte.Parse($"{stack.Pop()}");
            asm.Arch = (ArchType) byte.Parse($"{stack.Pop()}");
            asm.year = stack.Pop();
            asm.month = stack.Pop();

            stack.Pop(); // 0
            stack.Pop(); // 0

            return asm;
        }
        public static bool IsTag(string str)
        {
            if (str is null)
                return false;
            if (str.Length != 10)
                return false;
            if (!new Regex(@"(EF)([0-9]{2})([0-1])([0-9])([A-Z])([A-Z])[0-9]{2}").IsMatch(str))
                return false;
            return true;
        }

        public override string ToString() 
            => $"{ElementaryFlameTag}{Version:00}{(byte) Sign}{(byte) Arch}{year}{month}00";
    }
}