namespace flame.runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    // ReSharper disable VariableHidesOuterVariable
    public abstract class Instruction
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


        public static Instruction Summon(InsID id)
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
                    if (t.GetConstructors().First().GetParameters().Any())
                        return Activator.CreateInstance(t, args, null) as T;
                    return Activator.CreateInstance(t) as T;
                }
                var args = @class.GetConstructors().First().GetParameters().Select(@default).ToArray();
                var inst = Activate<Instruction>(@class, args);
                if (inst is { } block && block.ID == id)
                    return inst;
            }
            throw new InvalidOperationException($"Not found class for '{id}' operation.");
        }
    }
}