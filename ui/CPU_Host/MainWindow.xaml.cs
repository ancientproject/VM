namespace CPU_Host
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using JetBrains.Annotations;
    using MahApps.Metro.Controls;
    using vm.component;
    using vm.dev;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool IsLoading { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri($"file://{new FileInfo("./resource/icon.png").FullName}"));
            DataContext = this;
            new Thread(() =>
            {
                Thread.Sleep(5000);
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }).Start();
        }
        public void StartUpCPU()
        {
            var bus = HostContainer.Instance.bus;

            bus.Add(new Terminal(0x1));
            bus.Add(new AdvancedTerminal(0x2));

            var core = bus.Cpu;
            
            core.State.tc = Environment.GetEnvironmentVariable("FLAME_TRACE") == "1";
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

    public class HostContainer
    {
        public static readonly HostContainer Instance = new HostContainer();

        public Bus bus { get; set; }
    }
}
