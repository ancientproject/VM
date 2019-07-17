namespace ancient.runtime.emit.sys
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class EvaluationSegment
    {
        public byte Index { get; }
        public string Type { get; }

        public EvaluationSegment(byte index, string type)
        {
            Index = index;
            Type = type;
        }

        public (Instruction idx, Instruction type) OpCode
        {
            get
            {
                var index = new raw(ulong.Parse($"00{Index:X}0000", NumberStyles.AllowHexSpecifier));
                var type = new raw(ulong.Parse($"00{string.Join("", Encoding.ASCII.GetBytes(Type).Select(x => $"{x:X}"))}", 
                    NumberStyles.AllowHexSpecifier));
                return (index, type);
            }
        }


        public static EvaluationSegment Construct(in ulong idx, in ulong type)
        {
            var i = (byte)((idx & 0x0000_0000_00FF_0000) >> 16);
            var ss = string.Join("", Encoding.ASCII.GetString(BitConverter.GetBytes(type)).TrimEnd('\0').Reverse());
            return new EvaluationSegment(i, ss);
        }
    }
}