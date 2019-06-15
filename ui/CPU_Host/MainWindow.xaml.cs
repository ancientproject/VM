namespace CPU_Host
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using flame.runtime;
    using flame.runtime.emit;
    using JetBrains.Annotations;
    using MoreLinq.Extensions;
    using vm.dev;
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


        public static MainWindow Singleton { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Singleton = this;
            Icon = new BitmapImage(new Uri($"file://{new FileInfo("./resource/icon.png").FullName}"));
            DataContext = this;
            StartUpCPU();
            speed.ValueChanged += (o, args) =>
            {
                SpeedValue = (int)args.NewValue;
            };
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
                await Task.Delay(10);
            }
            for (var u = 0; u != block; u++)
            for (var v = 0; v != col; v++)
            {
                arr[u].TurnOff(v);
                await Task.Delay(10);
            }
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
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
                WriteToDebug($"]", Brushes.Gray);
                WriteToDebug($"{s}\r\n", Brushes.Red);
            }

            var bus = HostContainer.Instance.bus;
            bus.Cpu.OnError += exception =>
            {
                IsPLaying = false;
                OnPropertyChanged(nameof(IsPLaying));
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
                System.Windows.MessageBox.Show($"Access Violation Exception\n{exception.Message}\n{bus.Cpu.getStateOfCPU()}", $"CPU HALT", MessageBoxButton.OK, MessageBoxImage.Error);
                HostContainer.Instance.bus.Cpu.ResetMemory();
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
            bus.State.Registers = new WPFShadowCacheFactory();

            bus.Add(new LampBus(this, 
                block_01, block_02, block_03, block_04, block_05, block_06, block_07, block_08));

            var core = bus.Cpu;
            
            

            core.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";
            HostContainer.Instance.bus.State.instructionID = 0;
            HostContainer.Instance.bus.State.pc = 0;
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
                while (IsPLaying)
                {
                    Console.Beep();
                    await HostContainer.Instance.bus.Cpu.Step();
                    await Task.Delay((int)SpeedValue);
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
                    var dyn = FlameAssembly.LoadFrom(openFileDialog.FileName);
                    WriteSystemMessage($"Assembly '{dyn.Name}-{dyn.Tag}' load success.");
                    HostContainer.Instance.bus.State.Load(CastFromBytes(dyn.GetILCode()));
                    HostContainer.Instance.bus.State.instructionID = 0;
                    HostContainer.Instance.bus.State.pc = 0;
                    IsLoading = false;
                    OnPropertyChanged(nameof(IsLoading));
                });
                return;
            }
        }
        public static uint[] CastFromBytes(byte[] bytes)
        {
            if(bytes.Length % sizeof(uint) != 0)
                throw new Exception("invalid offset file.");
            return bytes.Batch(sizeof(uint)).Select(x => BitConverter.ToUInt32(x.ToArray())).Reverse().ToArray();
        }

        public void Step(object sender, EventArgs e) => 
            Task.Factory.StartNew(async () => await HostContainer.Instance.bus.Cpu.Step());

        public void SoftReset(object sender, EventArgs e)
        {
            IsPLaying = false;
            Task.Factory.StartNew(() => HostContainer.Instance.bus.Cpu.ResetCache());
        }
            
        public void HardReset(object sender, EventArgs e) => 
            Task.Factory.StartNew(() => HostContainer.Instance.bus.Cpu.ResetMemory());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
