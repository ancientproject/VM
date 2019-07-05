namespace CPU_Host
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using ancient.runtime.emit;
    using ancient.runtime.hardware;
    using MoreLinq.Extensions;
    using JetBrains.Annotations;
    using vm.component;

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool IsLoading { get; set; } = true;
        public bool IsPLaying { get; set; } = false;
        private LampBus.LampControl[] LED { get; set; }

        public static MainWindow Singleton { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            WriteSystemMessage("Booting bios...");
            if(MemoryManagement.FastWrite)
                WriteSystemMessage($"FastWrite: Enabled");
            Singleton = this;
            Icon = new BitmapImage(new Uri($"file://{new FileInfo("./resource/icon.png").FullName}"));
            DataContext = this;
            WriteSystemMessage($"Booting cpu...");
            StartUpCPU();
            speed.ValueChanged += (o, args) =>
            {
                SpeedValue = (int)args.NewValue;
            };
            LED = new []{block_01, block_02, block_03, block_04, block_05, block_06, block_07, block_08}
                .Select((x, i) => new LampBus.LampControl(i, x.Children.OfType<Ellipse>().ToArray())).ToArray();
            Task.Factory.StartNew(async () => { await WarmUpAllDiods(LED); });

        }

        public async Task WarmUpAllDiods(LampBus.LampControl[] arr)
        {
            WriteSystemMessage($"Booting LED device...");
            var block = arr.Length;
            var col = arr.First().LEDs.Length;
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
            {
                arr[u].TurnOn(v);
                await Task.Delay(1);
            }
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
            {
                arr[u].TurnOff(v);
                await Task.Delay(1);
            }
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            WriteSystemMessage($"Boot LED device success");
        }
        public void StartUpCPU()
        {
            void err(string s)
            {
                WriteToDebug($"[", Brushes.Gray);
                WriteToDebug(DateTime.Now.ToShortTimeString(), Brushes.DarkGoldenrod);
                WriteToDebug($"]", Brushes.Gray);
                WriteToDebug($"[", Brushes.Gray);
                WriteToDebug($"ERROR", Brushes.Red);
                WriteToDebug($"]: ", Brushes.Gray);
                WriteToDebug($"{s}\r\n", Brushes.Red);
            }

            var bus = HostContainer.Instance.bus;
            bus.cpu.OnError += exception =>
            {
                IsPLaying = false;
                OnPropertyChanged(nameof(IsPLaying));
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
                System.Windows.MessageBox.Show($"Access Violation Exception\n{exception.Message}\n{bus.cpu.getStateOfCPU()}", $"CPU HALT", MessageBoxButton.OK, MessageBoxImage.Error);
                ResetMemory(HostContainer.Instance.bus);
                err($"HALT {exception.Message}");
            };

            bus.State.OnError += err;
            bus.State.OnTrace += s =>
            {
                WriteToDebug($"[", Brushes.Gray);
                WriteToDebug(DateTime.Now.ToShortTimeString(), Brushes.DarkGoldenrod);
                WriteToDebug($"]", Brushes.Gray);
                WriteToDebug($"[", Brushes.Gray);
                WriteToDebug($"TRACE", Brushes.Gray);
                WriteToDebug($"]: ", Brushes.Gray);
                WriteToDebug($"{s}\r\n", Brushes.Gray);
            };

            bus.Add(new LampBus(this, 
                block_01, block_02, block_03, block_04, block_05, block_06, block_07, block_08));

            HostContainer.Instance.bus.State.iid = 0;
            HostContainer.Instance.bus.State.pc = 0;
        }

        public void ResetLED(object sender, EventArgs e)
        {
            var block = LED.Length;
            var col = LED.First().LEDs.Length;
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
                LED[u].TurnOff(v);
        }
        public void Stop(object sender, EventArgs e)
        {
            if(!IsPLaying || IsLoading)
                return;

            IsPLaying = false;
            OnPropertyChanged(nameof(IsPLaying));
        }
        private int SpeedValue { get; set; }
        public void Run(object sender, EventArgs e)
        {
            if(IsPLaying|| IsLoading)
                return;
            HostContainer.Instance.bus.State.halt = 0;
            IsPLaying = true;
            OnPropertyChanged(nameof(IsPLaying));
            
            Task.Factory.StartNew(async () =>
            {
                while (IsPLaying && HostContainer.Instance.bus.State.halt == 0)
                {
                    HostContainer.Instance.bus.cpu.Step();
                    await Task.Delay(10);
                }
            });
        }

        public void WriteSystemMessage(string message)
        {
            WriteToDebug($"[", Brushes.Gray);
            WriteToDebug(DateTime.Now.ToShortTimeString(), Brushes.DarkGoldenrod);
            WriteToDebug($"]", Brushes.Gray);
            WriteToDebug($"[", Brushes.Gray);
            WriteToDebug($"SYS", Brushes.MediumPurple);
            WriteToDebug($"]: ", Brushes.Gray);
            WriteToDebug($"{message}\r\n", Brushes.MediumPurple);
        }

        public void WriteToDebug(string message, Brush b)
        {
            outputLog.Dispatcher.Invoke(() =>
            {
                var tr = new TextRange(outputLog.Document.ContentEnd, outputLog.Document.ContentEnd)
                {
                    Text = message
                };
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, b);
                if (outputLog.Document.Blocks.Count > 25)
                    outputLog.Document.Blocks.Remove(outputLog.Document.Blocks.FirstBlock);
                outputLog.ScrollToEnd();
            });
        }

        public void Load(object sender, EventArgs e)
        {
            if(IsPLaying || IsLoading)
                return;
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Flame Binary(*.flx;*.dlx)|*.FLX;*.DLX",
                Title = "Select flame binary."
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WriteSystemMessage($"Loading '../{System.IO.Path.GetFileName(openFileDialog.FileName)}'");
                IsLoading = true;
                OnPropertyChanged(nameof(IsLoading));
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(1000);
                    var dyn = AncientAssembly.LoadFrom(openFileDialog.FileName);
                    WriteSystemMessage($"Assembly '{dyn.Name}-{dyn.Tag}' load success.");
                    HostContainer.Instance.bus.State.Load("<execute>",CastFromBytes(dyn.GetILCode()));
                    HostContainer.Instance.bus.State.iid = 0;
                    HostContainer.Instance.bus.State.pc = 0;
                    IsLoading = false;
                    OnPropertyChanged(nameof(IsLoading));
                });
                return;
            }
        }
        public static ulong[] CastFromBytes(byte[] bytes)
        {
            if (bytes.Length % sizeof(ulong) == 0)
                return bytes.Batch(sizeof(ulong)).Select(x => BitConverter.ToUInt64(x.ToArray())).Reverse().ToArray();
            if (bytes.Length % sizeof(uint) == 0)
                return bytes.Batch(sizeof(uint)).Select(x => BitConverter.ToUInt64(x.ToArray())).Reverse().ToArray();
            throw new Exception("invalid offset file.");
        }

        public void ResetMemory(object sender, EventArgs e)
        {
            IsPLaying = false;
            OnPropertyChanged(nameof(IsPLaying));
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            ResetMemory(HostContainer.Instance.bus);
            WriteSystemMessage("RESET");
        }

        public static void ResetMemory(Bus bus)
        {
            if (bus.Find(0x45) is BIOS bios)
            {
                // enable bios allow read internal memory
                bus.State.southFlag = true;
                // disable bios_guard
                bios.write(0xA4, 0x0);
                // disable bios allow read internal memory
                bus.State.southFlag = false;
                // call clear RAM
                bios.write(0xC, 0x0);
                // enable bios allow read internal memory
                bus.State.southFlag = true;
                // enable bios_guard
                bios.write(0xA4, 0x1);
                // disable bios allow read internal memory
                bus.State.southFlag = false;
            }
        }

        public void Step(object sender, EventArgs e) => 
            Task.Factory.StartNew(HostContainer.Instance.bus.cpu.Step);

            
        public void HardReset(object sender, EventArgs e) => 
            Task.Factory.StartNew(() => ResetMemory(HostContainer.Instance.bus));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
