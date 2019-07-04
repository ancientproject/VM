using System;
using System.Windows;

namespace CPU_Host
{
    using ancient.runtime.hardware;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IntToCharConverter.Register<char>();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MessageBox.Show($"CPU HALT", $"{args.ExceptionObject}", MessageBoxButton.OK, MessageBoxImage.Error);
                HostContainer.Instance.bus.State.halt = 1;
                Environment.Exit(-1);
            };
        }
    }
}
