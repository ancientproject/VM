namespace ancient.runtime
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Field)]
    public class OpCodeAttribute : Attribute
    {
        private readonly bool _isIgnore;
        public readonly short OpCode;
        public OpCodeAttribute(short op) => OpCode = op;
        public OpCodeAttribute(bool isIgnore)
        {
            _isIgnore = isIgnore;
            OpCode = short.MinValue;
        }
    }

    public class UnfCode : OpCodeAttribute
    {
        public UnfCode() : base(true)
        {
        }
    }

    public static class InsIDEx
    {
        public static short getOpCode(this IID id)
        {
            var member = typeof(IID).GetMember(id.ToString()).FirstOrDefault(m => m.DeclaringType == typeof(IID));

            if(member is null)
                throw new InvalidOperationException();


            var attr = member.GetCustomAttributes(typeof(OpCodeAttribute), false).FirstOrDefault() as OpCodeAttribute;

            if(attr is null)
                throw new 
                    InvalidOperationException(
                        $"Field '{id}' of type '{nameof(IID)}' not found '{nameof(OpCodeAttribute)}' attribute. ");

            return attr.OpCode;
        }
        public static IID getInstruction(this ushort id)
        {
            var member = typeof(IID).GetMembers().Where(m => m.DeclaringType == typeof(IID));

            if (member is null)
                throw new InvalidOperationException();


            var attr = member.Select(x => new { x , atr= x.GetCustomAttributes(typeof(OpCodeAttribute), true).FirstOrDefault() as OpCodeAttribute });

            if (attr is null)
                throw new
                    InvalidOperationException(
                        $"Field '{id}' of type '{nameof(IID)}' not found '{nameof(OpCodeAttribute)}' attribute. ");

            foreach (var op in attr)
            {
                if(op.atr is null)
                    continue;
                if (op.atr.OpCode == id)
                    return (IID)Enum.Parse(typeof(IID), op.x.Name, true);
            }

            return IID.halt;
        }
    }

    public class IIDAliase : Attribute
    {
        public readonly IID[] IID;

        public IIDAliase(params IID[] iid) => IID = iid;
    }
    public enum IID : short
    {
        [OpCode(0x0A)] warm,
        [OpCode(0x0D)] halt,

        [OpCode(0x01)] ldi,
        [OpCode(0x01)] ldx,

        [OpCode(0x0F)] mva, [OpCode(0x0F)] mvd,
        [OpCode(0x0F)] mvx, [OpCode(0xA2)] mvt,

        [OpCode(0x03)] swap,

        [OpCode(0x08)] ref_t , [OpCode(0x08)] jump_t, 
        [OpCode(0x08)] jump_e, [OpCode(0x08)] jump_g,
        [OpCode(0x08)] jump_u, [OpCode(0x08)] jump_y,

        [UnfCode] mvj, [UnfCode] raw,

        [OpCode(0xA0)] orb,
        [OpCode(0xA1)] pull,
        [OpCode(0xAA)] val,

        [OpCode(0xC1)] brk_s,
        [OpCode(0xC1)] brk_n,
        [OpCode(0xC1)] brk_a,

        [OpCode(0xA4)] inv,
        [OpCode(0xA5)] sig, 
        [OpCode(0xA6)] ret,

        [OpCode(0xB1)] inc,
        [OpCode(0xB2)] dec,

        // 1x, abs, acos, atan, acosh, atanh, asin, asinh, cbrt, cell, cos, cosh, flr, exp, log, log10, tan, tanh, trc, bitd, biti
        // 2x, atan2, min, max

        [OpCode(0xD0)] abs, 
        [OpCode(0xD1)] acos, 
        [OpCode(0xD2)] atan,
        [OpCode(0xD3)] acosh, 
        [OpCode(0xD4)] atanh, 
        [OpCode(0xD5)] asin, 
        [OpCode(0xD6)] asinh,
        [OpCode(0xD7)] cbrt,
        [OpCode(0xD8)] cell,
        [OpCode(0xD9)] cos,
        [OpCode(0xDA)] cosh,
        [OpCode(0xDB)] flr,
        [OpCode(0xDC)] exp,
        [OpCode(0xDD)] log,
        [OpCode(0xDE)] log10,
        [OpCode(0xDF)] tan,
        [OpCode(0xE0)] tanh,
        [OpCode(0xE1)] trc,
        [OpCode(0xE2)] bitd,
        [OpCode(0xE3)] biti, 

        [OpCode(0xE4)] atan2,
        [OpCode(0xE5)] min,
        [OpCode(0xE6)] max,

        [OpCode(0xE7)] pow, 
        [OpCode(0xE8)] sqrt,


        [OpCode(0xE9)] add, 
        [OpCode(0xEA)] sub,
        [OpCode(0xEB)] div, 
        [OpCode(0xEC)] mul,
        
    }
}