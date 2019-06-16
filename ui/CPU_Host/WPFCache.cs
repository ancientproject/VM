namespace CPU_Host
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Media;
    using vm.component;

    public class WPFCache : Cache
    {
        private ushort _iid = 0xFF;
        private ushort _r1 = 0xFF;
        private ushort _r2 = 0xFF;
        private ushort _r3 = 0xFF;
        private ushort _u1 = 0xFF;
        private ushort _u2 = 0xFF;
        private ushort _x1 = 0xFF;
        private ushort _x2 = 0xFF;
        private ulong _pc = 0xFF;

        public override ushort IID
        {
            get => _iid;
            set
            {
                _iid = value;
                var state = HostContainer.Instance.bus.State;
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.IC.Content = $"IC: 0x{value:X8}"; });
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.CurAddr.Content = $"CA: 0x{state.curAddr:X}"; });
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.LastAddr.Content = $"LA: 0x{state.lastAddr:X}"; });
            }
        }

        public override ulong PC
        {
            get => _pc;
            set
            {
                _pc = value;
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.PC.Content = $"PC: 0x{value:X8}"; });
            }
        }

        private void Trigger(ListBoxItem el, ushort value)
        {
            el.Content = $"0x{value:X2}";
            if(value == 0xFF || value == 0x0 || value == 0xF)
                el.Foreground = Brushes.Red;
            else
                el.Foreground = Brushes.GreenYellow;
        }
        public override ushort r1
        {
            get => _r1;
            set
            {
                _r1 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[0], value);
                });
            }
        }
        public override ushort r2
        {
            get => _r2;
            set
            {
                _r2 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[1], value);
                });
            }
        }
        public override ushort r3
        {
            get => _r3;
            set
            {
                _r3 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[2], value);
                });
            }
        }
        public override ushort u1
        {
            get => _u1;
            set
            {
                _u1 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[3], value);
                });
            }
        }
        public override ushort u2
        {
            get => _u2;
            set
            {
                _u2 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[4], value);
                });
            }
        }
        public override ushort x1
        {
            get => _x1;
            set
            {
                _x1 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[5], value);
                });
            }
        }
        public override ushort x2
        {
            get => _x2;
            set
            {
                _x2 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    Trigger(MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[6], value);
                });
            }
        }
    }
}