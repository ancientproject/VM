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
    public enum IID : short
    {
        [OpCode(0xA)] warm,
        [OpCode(0xD)] halt,

        [OpCode(0x01)] ldi,
        [OpCode(0x01)] ldx,

        [OpCode(0x02)] add, [OpCode(0x04)] sub,
        [OpCode(0x06)] div, [OpCode(0x05)] mul,
        [OpCode(0x07)] pow, [OpCode(0x07)] sqrt,

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
        [OpCode(0xB2)] dec
    }
}