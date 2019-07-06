namespace CPU_Host
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Media;
    using MoreLinq;
    using vm.component;

    public class HostContainer
    {
        private static HostContainer h { get; set; }

        public static HostContainer Instance
        {
            get
            {
                if(h is null)
                    return h = new HostContainer();
                return h;
            }
        }

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
                    try
                    {
                        s.IC.Content = $"IC: 0x{state.iid:X8}";
                        s.CurAddr.Content = $"CA: 0x{state.curAddr:X}";
                        s.LastAddr.Content = $"LA: 0x{state.lastAddr:X}";
                        s.PC.Content = $"PC: 0x{state.pc:X8}";
                        _ = new[] {state.r1, state.r2, state.r3, state.u1, state.u2, state.x1, state.x2}
                            .Select((val, index) => (val, index))
                            .Pipe(x => Trigger(s.regBox.Items.OfType<ListBoxItem>().ToArray()[x.index], x.val))
                            .ToArray();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                });
            }
        }
    }
}