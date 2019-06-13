namespace flame.runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

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

        public int Assembly()
        {
            OnCompile();
            Func<int> Shift = ShiftFactory.Create(28).Shift;

            //var op = (((OPCode & 0xF0) >> 4) << Shift()) | (((OPCode & 0xF) >> 0) << Shift());
            var op =  (OPCode << Shift());
            var rr1 = (_r1 << Shift());
            var rr2 = (_r2 << Shift());
            var rr3 = (_r3 << Shift());
            var ru1 = (_u1 << Shift());
            var ru2 = (_u2 << Shift());
            var rx1 = (_x1 << Shift());
            var rx2 = (_x2 << Shift());

            return  op | rr1 | 
                   rr2 | rr3 | 
                   ru1 | ru2 | 
                   rx1 | rx2;
        }



        public static implicit operator int(Instruction i) => i.Assembly();
        public static implicit operator uint(Instruction i) => (uint)i.Assembly();


        public override byte[] GetBodyILBytes() => BitConverter.GetBytes(Assembly());
        public override string ToString() => $"{ID} [{string.Join(" ", GetBodyILBytes().Select(x => x.ToString("X2")))}]";

        public List<string> Single = new List<string>(new []{"28"});

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

        public static readonly object Guarder = new object();

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

    public class ShiftFactory : IShifter
    {
        private int prev;
        private int index;

        private ShiftFactory() {}

        public static IShifter Create(int bitIndex) => new ShiftFactory {index = bitIndex};

        public int Shift()
        {
            prev = index;
            index -= 4;
            if (index < 0) index = 0;
            return prev;
        }
    }
    public interface IShifter
    {
        int Shift();
    }
} 