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

    public static class InsIDEx
    {
        public static short getOpCode(this InsID id)
        {
            var member = typeof(InsID).GetMember(id.ToString()).FirstOrDefault(m => m.DeclaringType == typeof(InsID));

            if(member is null)
                throw new InvalidOperationException();


            var attr = member.GetCustomAttributes(typeof(OpCodeAttribute), false).FirstOrDefault() as OpCodeAttribute;

            if(attr is null)
                throw new 
                    InvalidOperationException(
                        $"Field '{id}' of type '{nameof(InsID)}' not found '{nameof(OpCodeAttribute)}' attribute. ");

            return attr.OpCode;
        }
        public static InsID getInstruction(this ushort id)
        {
            var member = typeof(InsID).GetMembers().Where(m => m.DeclaringType == typeof(InsID));

            if (member is null)
                throw new InvalidOperationException();


            var attr = member.Select(x => new { x , atr= x.GetCustomAttributes(typeof(OpCodeAttribute), false).FirstOrDefault() as OpCodeAttribute });

            if (attr is null)
                throw new
                    InvalidOperationException(
                        $"Field '{id}' of type '{nameof(InsID)}' not found '{nameof(OpCodeAttribute)}' attribute. ");

            foreach (var op in attr)
            {
                if(op.atr is null)
                    continue;
                if (op.atr.OpCode == id)
                    return (InsID)Enum.Parse(typeof(InsID), op.x.Name, true);
            }

            return InsID.halt;
        }
    }
    public enum InsID : short
    {
        [OpCode(0xA)]
        warm,
        [OpCode(0x1)]
        loadi,
        [OpCode(0x1)]
        loadi_x,
        [OpCode(0x2)]
        add,
        [OpCode(0x4)]
        sub,
        [OpCode(0x6)]
        div,
        [OpCode(0x5)]
        mul,
        [OpCode(0x7)]
        pow,
        [OpCode(0x7)]
        sqrt,
        [OpCode(0xF)]
        push_a,
        [OpCode(0xF)]
        push_d,
        [OpCode(0xF)]
        push_x,
        [OpCode(0x3)]
        swap,

        [OpCode(0x8)]
        ref_t,
        [OpCode(0x8)]
        jump_t,
        [OpCode(0x8)]
        jump_e,
        [OpCode(0x8)]
        jump_g,
        [OpCode(0x8)]
        jump_u,
        [OpCode(0x8)]
        jump_y,

        [OpCode(true)]
        mov_d, // todo
        [OpCode(true)]
        push_j,

        [OpCode(0xA0)]
        stage_n,
        [OpCode(0xA1)]
        loadi_s,
        [OpCode(0xAA)]
        n_value,
        [OpCode(true)]
        raw,

        [OpCode(0xD)]
        halt
    }
}