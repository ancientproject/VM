namespace CPU_Host
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using ancient.runtime;
    using ancient.runtime.exceptions;
    using ancient.runtime.hardware;

    public class LampBus : Device
    {
        private readonly Window _window;
        private readonly LampControl[] _stack;

        public class LampControl
        {
            public Ellipse[] LEDs { get; }
            public int StackIndex { get; set; }

            public LampControl(int i, params Ellipse[] el)
            {
                StackIndex = i;
                LEDs = el;
            }

            public void TurnOff(int index)
            {
                if(LEDs.Length <= index)
                    throw new CorruptedMemoryException($"LED 0x{index:X} not found in 0x{StackIndex:X} stack.");
                LEDs[index].Dispatcher.Invoke(() => { LEDs[index].Fill = Brushes.Gray; });
            }
            public void TurnOn(int index)
            {
                if(LEDs.Length <= index)
                    throw new CorruptedMemoryException($"LED 0x{index:X} not found in 0x{StackIndex:X} stack.");
                LEDs[index].Dispatcher.Invoke(() => { LEDs[index].Fill = Brushes.Yellow; });
            }
        }

        public LampBus(Window window, params Grid[] stack) : base(0xB, "LED-Device")
        {
            _window = window;
            _stack = stack.Select((v, i) => new LampControl(i, v.Children.OfType<Ellipse>().ToArray())).ToArray();
        }

        private void Manage(int u1, int u2, bool isPowerOff)
        {
            if(_stack.Length <= u1)
                throw new CorruptedMemoryException($"LED stack 0x{u1:X} not found.");
            
            if(isPowerOff)
                _stack[u1].TurnOff(u2);
            else
                _stack[u1].TurnOn(u2);
        }

        [ActionAddress(0xD)]
        public void Light(char reg) => Manage((reg & 0xF0) >> 4, reg & 0xF, false);
        [ActionAddress(0xE)]
        public void PowerOff(char reg) => Manage((reg & 0xF0) >> 4, reg & 0xF, true);
    }
}