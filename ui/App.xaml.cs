using System;
using System.Windows;
using vm.dev.Internal;

namespace CPU_Host
{
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
