namespace vm.dev
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using ancient.runtime;
    using component;
    using UnitsNet;
    using UnitsNet.Units;

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
                var freq = new Frequency(state.GetHertz(), FrequencyUnit.Hertz);
                var type = FrequencyUnit.Hertz;

                if (freq.Gigahertz > 0.1)
                    type = FrequencyUnit.Gigahertz;
                else if (freq.Megahertz > 0.1)
                    type = FrequencyUnit.Megahertz;
                else if (freq.Kilohertz > 0.1)
                    type = FrequencyUnit.Kilohertz;

                Console.Title = $"[vm_host] {freq.ToUnit(type).ToString(CultureInfo.InvariantCulture)}";
                await Task.Delay(200);
            }
        }
    }
}