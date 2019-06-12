namespace flame.runtime
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    [DebuggerDisplay("{ToString()}")]
    public abstract class Instruction : OpCode
    {
        public InsID ID { get; }
        public short OPCode { get; }

        protected Instruction(InsID id)
        {
            ID = id;
            OPCode = id.getOpCode();
        }

        public int Assembly() => (OPCode << Shift(true)) 
          | (_r1 << Shift()) | (_r2 << Shift()) 
          | (_r3 << Shift()) | (_u1 << Shift()) 
          | (_u2 << Shift()) | (_x1 << Shift())
          | (_x2 << Shift());



        public static implicit operator int(Instruction i) => i.Assembly();
        public static implicit operator uint(Instruction i) => (uint)i.Assembly();


        public override byte[] GetBodyILBytes() => BitConverter.GetBytes(Assembly());
        public override string ToString() => $"{ID} [{string.Join(" ", GetBodyILBytes().Select(x => x.ToString("X2")))}]";

        public int StartShirtIndex = 28;

        public void SetRegisters(byte r1 = 0, byte r2 = 0, byte r3 = 0, byte u1 = 0, byte u2 = 0, byte x1 = 0,
            byte x2 = 0)
        {
            _r1 = r1;
            _r2 = r2;
            _r3 = r3;
            _u1 = u1;
            _u2 = u2;
            _x1 = x1;
            _x2 = x2;
        }

        private byte _r1, _r2, _r3, _u1, _u2, _x1, _x2;

        protected abstract void OnCompile();

        private int Shift(bool start = false)
        {
            if (start)
                StartShirtIndex = 28;
            var index = StartShirtIndex;
            StartShirtIndex =- 4;
            if (StartShirtIndex < 0)
                StartShirtIndex = 0;
            return index;
        }

        public static Instruction Summon(InsID id, params object[] args)
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
    }
}