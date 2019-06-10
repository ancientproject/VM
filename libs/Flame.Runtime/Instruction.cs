namespace flame.runtime
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using emit;
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

        public abstract ulong Assembly();



        public static implicit operator ulong(Instruction i) => i.Assembly();
        public static implicit operator uint(Instruction i) => (uint)i.Assembly();


        public override byte[] GetBodyILBytes() => BitConverter.GetBytes(Assembly());
        public override string ToString() => $"{ID} [{string.Join(" ", GetBodyILBytes().Select(x => x.ToString("X2")))}]";


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