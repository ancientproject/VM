namespace CPU_Host
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using flame.runtime.exceptions;
    using vm.dev;
    using vm.dev.Internal;

    public class LampBus : IDevice
    {
        private readonly Window _window;
        private readonly LampControl[] _stack;

        public string Name => "LampDevice";
        public short StartAddress => 0xB;

        public class LampControl
        {
            public Ellipse[] Diods { get; }
            public int StackIndex { get; set; }

            public LampControl(int i, params Ellipse[] el)
            {
                StackIndex = i;
                Diods = el;
            }

            public void TurnOff(int index)
            {
                if(Diods.Length < index)
                    throw new CorruptedMemoryException($"lamp module not found.");
                Diods[index].Dispatcher.Invoke(() => { Diods[index].Fill = Brushes.Gray; });
            }
            public void TurnOn(int index)
            {
                if(Diods.Length < index)
                    throw new CorruptedMemoryException($"lamp module not found.");
                Diods[index].Dispatcher.Invoke(() => { Diods[index].Fill = Brushes.Yellow; });
            }
        }

        public LampBus(Window window, params Grid[] stack)
        {
            _window = window;
            _stack = stack.Select((v, i) => new LampControl(i, v.Children.OfType<Ellipse>().ToArray())).ToArray();
        }

        private void Manage(int u1, int u2, bool isPowerOff)
        {
            if(_stack.Length < u1)
                throw new CorruptedMemoryException("lamp bus not found.");
            
            if(isPowerOff)
                _stack[u1].TurnOff(u2);
            else
                _stack[u1].TurnOn(u2);
        }

        [ActionAddress(0xD)]
        public void Light(char reg) => Manage(reg & 0xF0 >> 4, reg & 0xF, false);
        [ActionAddress(0xE)]
        public void PowerOff(char reg) => Manage(reg & 0xF0 >> 4, reg & 0xF, true);



        public void write(int address, int data) => (this as IDevice).WriteMemory(address, data);

        public int read(int address) => (this as IDevice).read(address);

        public void Init()
        {

        }

        public void Shutdown()
        {

        }
    }
}