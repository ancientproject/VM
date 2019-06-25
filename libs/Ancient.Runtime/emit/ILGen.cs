namespace ancient.runtime.emit
{
    using System.Collections.Generic;
    using System.Linq;

    public class ILGen
    {
        private readonly DynamicAssembly _asm;
        private readonly Stack<OpCode> ilStack = new Stack<OpCode>();
        internal ILGen(DynamicAssembly asm) => _asm = asm;

        public void Emit(params OpCode[] instruction)
        {
            foreach (var i in instruction) ilStack.Push(i);
        }

        public bool Any() => ilStack.Any();


        public OpCode[] Load() => ilStack.ToArray();
    }
}