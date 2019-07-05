namespace CPU_Host
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Media;
    using MoreLinq;
    using vm.component;

    public class HostContainer
    {
        public static readonly HostContainer Instance = new HostContainer();

        public HostContainer()
        {
            bus = new Bus();
            bus.State = new HostState(bus);
        }

        public Bus bus { get; set; }
    }

    public class HostState : State
    {
        private ulong _step {get;set;}
        public HostState(Bus bus) : base(bus) { }

        private void Trigger(ListBoxItem el, ushort value)
        {
            el.Content = $"0x{value:X2}";
            if(value == 0xFF || value == 0x0 || value == 0xF)
                el.Foreground = Brushes.Red;
            else
                el.Foreground = Brushes.GreenYellow;
        }

        public override ulong step
        {
            get => _step;
            set
            {
                _step = value;
                var state = this;
                var s = MainWindow.Singleton;
                s.Dispatcher.Invoke(() =>
                {
                    s.IC.Content = $"IC: 0x{iid:X8}";
                    s.CurAddr.Content = $"CA: 0x{curAddr:X}";
                    s.LastAddr.Content = $"LA: 0x{lastAddr:X}";
                    s.PC.Content = $"PC: 0x{pc:X8}";
                    _ = new[] {r1, r2, r3, u1, u2, x1, x2}
                        .Select((value, index) => (value, index))
                        .Pipe(x => Trigger(s.regBox.Items.OfType<ListBoxItem>().ToArray()[x.index], x.value))
                        .ToArray();
                });
            }
        }
    }
}