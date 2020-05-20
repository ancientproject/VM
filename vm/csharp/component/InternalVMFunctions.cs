namespace vm.component
{
    using System.Text;
    using ancient.runtime.emit.sys;
    using static System.Console;

    public static class InternalVMFunctions
    {
        private static Bus _bus { get; set; }


        public static void Setup(Bus bus)
        {
            _bus = bus;
            Module.Global.Add("sys->DumpCallStack()", typeof(InternalVMFunctions).GetMethod("DumpCallStack"));
        }



        public static void DumpCallStack()
        {
            var callStack = _bus.State.CallStack;
            var frames = callStack.GetFrames();
            var text = new StringBuilder();

            foreach (var frame in frames) 
                WriteLine($"at {frame}");

            WriteLine(text.ToString());
        }
    }
}