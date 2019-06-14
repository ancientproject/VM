namespace CPU_Host
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using flame.runtime;
    using JetBrains.Annotations;
    using vm.component;
    using vm.dev;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool IsLoading { get; set; } = true;
        public static MainWindow Singleton { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Singleton = this;
            Icon = new BitmapImage(new Uri($"file://{new FileInfo("./resource/icon.png").FullName}"));
            DataContext = this;
            new Thread(() =>
            {
                Thread.Sleep(5000);
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
                
            }).Start();
            StartUpCPU();
            var arr = new []{block_01, block_02, block_03, block_04, block_05, block_06, block_07, block_08}
                .Select((x, i) => new LampBus.LampControl(i, x.Children.OfType<Ellipse>().ToArray())).ToArray();
            Task.Factory.StartNew(async () => { await WarmUpAllDiods(arr); });

        }

        public async Task WarmUpAllDiods(LampBus.LampControl[] arr)
        {
            var block = arr.Length;
            var col = arr.First().Diods.Length;
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
            {
                arr[u].TurnOn(v);
                await Task.Delay(100);
            }
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
            {
                arr[u].TurnOff(v);
                await Task.Delay(100);
            }
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3000);
                while (true)
                {
                    Console.Beep();
                    await HostContainer.Instance.bus.Cpu.Step();
                    await Task.Delay(600);
                }
            });
        }
        public void StartUpCPU()
        {
            var bus = HostContainer.Instance.bus;

            bus.State.Registers = new WPFShadowCacheFactory();

            bus.Add(new LampBus(this, 
                block_01, block_02, block_03, block_04, block_05, block_06, block_07, block_08));

            var core = bus.Cpu;
            
            

            core.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";

            core.State.Load(new ref_t(0x6));
            core.State.Load(new loadi(0x2,  0x11));
            core.State.Load(new loadi(0x3,  0x1));

            foreach (var i in Enumerable.Range(0, 5))
            {
                core.State.Load(new sum(0x4,  0x2, 0x3));
                core.State.Load(new sum(0x2,  0x2, 0x3));
                core.State.Load(new push_d(0xB, 0xD, 0x4));
                core.State.Load(new push_d(0xB, 0xE, 0x4));
            }



            core.State.Load(new jump_t(0x6));
        }


        public void SwitchRun()
        {

        }

        public void Step(object sender, EventArgs e) => 
            Task.Factory.StartNew(async () => await HostContainer.Instance.bus.Cpu.Step());
        public void SoftReset(object sender, EventArgs e) => 
            Task.Factory.StartNew(() => HostContainer.Instance.bus.Cpu.ResetCache());
        public void HardReset(object sender, EventArgs e) => 
            Task.Factory.StartNew(() => HostContainer.Instance.bus.Cpu.ResetMemory());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class WPFShadowCacheFactory : ShadowCache<Cache>
    {
        public Cache L1 { get; set; } = new WPFCache();

        public Cache L2 { get; set; } = new Cache();

        public void Reflect()
        {
            L2 = (Cache)L1.Clone();
        }
    }

    public class WPFCache : Cache
    {
        private ushort _iid;
        private ushort _r1;
        private ushort _r2;
        private ushort _r3;
        private ushort _u1;
        private ushort _u2;
        private ushort _x1;
        private ushort _x2;
        private ulong _pc;

        public override ushort IID
        {
            get => _iid;
            set
            {
                _iid = value;
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.IC.Content = $"0x{value:X8}"; });
            }
        }

        public override ulong PC
        {
            get => _pc;
            set
            {
                _pc = value;
                MainWindow.Singleton.Dispatcher.Invoke(() => { MainWindow.Singleton.PC.Content = $"0x{value:X8}"; });
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

    public class HostContainer
    {
        public static readonly HostContainer Instance = new HostContainer();

        public Bus bus { get; set; } = new Bus();
    }
}
