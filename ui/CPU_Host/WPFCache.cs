namespace CPU_Host
{
    using System.Linq;
    using System.Windows.Controls;
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
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.IC.Content = $"IC: 0x{value:X8}"; });
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

        public override ushort r1
        {
            get => _r1;
            set
            {
                _r1 = value;
                MainWindow.Singleton.Dispatcher.Invoke(() =>
                {
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[0].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[1].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[2].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[3].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[4].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[5].Content = $"0x{value:X2}";
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
                    MainWindow.Singleton.regBox.Items.OfType<ListBoxItem>().ToArray()[6].Content = $"0x{value:X2}";
                });
            }
        }
    }
}