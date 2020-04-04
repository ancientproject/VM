namespace vm.dev
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using ancient.runtime;
    using component;

    public class HwndWindowsHookDevice : Device
    {
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        public HwndWindowsHookDevice() : base(0xFF4, "<hwnd-windows>")
        {
        }

        public override void warmUp()
        {
            if(!AppFlag.GetVariable("DISPLAY_FREQUENCY"))
                return;
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;
            this.state = ((this as IBusGate).getBus() as Bus)?.State;

            if (this.state is null)
                ThrowMemoryRead(0x0, startAddress, new Exception("Cannot read bus connecter."));

            Task.Run(Render, tokenSource.Token);
        }

        private State state { get; set; }

        public override void shutdown() => tokenSource.Cancel();

        private async Task Render()
        {
            while (!tokenSource.IsCancellationRequested)
            {
                var value = state.GetHertz();
                var formatted = value > 100 ? $"{MathF.Round(value / 1000f, 2)} GHz" : $"{value} Hz";

                Console.Title = $"[vm_host] {formatted}";
                await Task.Delay(200);
            }
        }
    }
}