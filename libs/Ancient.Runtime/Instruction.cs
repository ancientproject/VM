namespace ancient.runtime
{
    using System;
    using System.Linq;
    using System.Reflection;

    public abstract class Instruction : OpCode
    {
        public IID ID { get; protected set; }
        public ushort OPCode { get; protected set; }

        private byte _r1, _r2, _r3, _u1, _u2, _x1, _x2, _x3;

        protected Instruction() { }
        protected Instruction(IID id)
        {
            ID = id;
            OPCode = (ushort)id.getOpCode();
        }

        public void SetOpCode(IID id)
        {
            ID = id;
            OPCode = (ushort)id.getOpCode();
        }

        public virtual ulong Assembly()
        {
            OnCompile();
            Func<int> Shift = ShiftFactory.Create(36);
            
            var op1 = ((OPCode & 0xF0UL) >> 4) << Shift();
            var op2 = ((OPCode & 0x0FUL) >> 0) << Shift();
            var rr1 = (ulong)_r1 << Shift();
            var rr2 = (ulong)_r2 << Shift();
            var rr3 = (ulong)_r3 << Shift();
            var ru1 = (ulong)_u1 << Shift();
            var ru2 = (ulong)_u2 << Shift();
            var rx1 = (ulong)_x1 << Shift();
            var rx2 = (ulong)_x2 << Shift();
            var rx3 = (ulong)_x3 << Shift();
            return op1 | op2 | rr1 |
                   rr2 | rr3 | 
                   ru1 | ru2 | 
                   rx1 | rx2 | rx3;
        }

        public override byte[] GetBodyILBytes() => BitConverter.GetBytes(Assembly()).Reverse().ToArray();
        public override byte[] GetMetaDataILBytes() => !HasMetadata() ? Array.Empty<byte>() : metadataBytes;

        protected internal virtual byte[] metadataBytes { get; } = Array.Empty<byte>();
        protected abstract void OnCompile();

        #region serviced

        public static implicit operator ulong(Instruction i) => i.Assembly();

        public override string ToString() => $"{ID} [{string.Join(" ", GetBodyILBytes().Select(x => x.ToString("X2")))}]";

        public void Construct(byte r1 = 0, byte r2 = 0, byte r3 = 0, byte u1 = 0, byte u2 = 0, byte x1 = 0,
            byte x2 = 0, byte x3 = 0) => SetRegisters(r1, r2, r3, u1, u2, x1, x2, x3);
        [Obsolete("use Construct")]
        public void SetRegisters(byte r1 = 0, byte r2 = 0, byte r3 = 0, byte u1 = 0, byte u2 = 0, byte x1 = 0, byte x2 = 0, byte x3 = 0)
        {
            _r1 = r1; 
            _r2 = r2;
            _r3 = r3; 
            _u1 = u1;
            _u2 = u2; 
            _x1 = x1;
            _x2 = x2;
            _x3 = x3;
        }
        public static Instruction Summon(IID id, params object[] args)
        {
            var currentAsm = typeof(Instruction).Assembly;
            var classes =
                from type in currentAsm.GetTypes()
                where type.IsClass
                where !type.IsAbstract
                where type.IsSubclassOf(typeof(Instruction))
                select type;

            foreach (var @class in classes)
            {
                static object @default(ParameterInfo t) => t.ParameterType.IsValueType ? 
                    Activator.CreateInstance(t.ParameterType) : 
                    null;
                static T Activate<T>(Type t, object[] args) where T : class
                {
                    var @params = t.GetConstructors().First().GetParameters();
                    if (!@params.Any() || !args.Any())
                        return Activator.CreateInstance(t) as T;
                    if (args.Length == @params.Length)
                        return Activator.CreateInstance(t, args, null) as T;
                    return default;
                }

                if(!args.Any())
                    args = @class.GetConstructors().First().GetParameters().Select(@default).ToArray();
                var inst = Activate<Instruction>(@class, args);
                if (inst is { } block && block.ID == id)
                    return inst;
            }
            throw new InvalidOperationException($"Not found class for '{id}' operation.");
        }

        #endregion
    }
} 