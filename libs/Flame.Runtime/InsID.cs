namespace flame.runtime
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Field)]
    public class OpCodeAttribute : Attribute
    {
        public readonly short OpCode;
        public OpCodeAttribute(short op) => OpCode = op;
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
    }
    public enum InsID : short
    {
        [OpCode(0xA)]
        warm,
        [OpCode(0x1)]
        loadi,
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
        [OpCode(0xF)]
        push_a,
        [OpCode(0xF)]
        push_d,
        [OpCode(0x3)]
        swap,

        [OpCode(0x8)]
        ref_t,
        [OpCode(0x8)]
        jump_t,

        mov_d, // todo

        [OpCode(0xD)]
        halt
    }
}