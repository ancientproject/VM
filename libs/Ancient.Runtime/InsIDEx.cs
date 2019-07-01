namespace ancient.runtime
{
    using System;
    using System.Linq;

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
}