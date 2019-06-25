namespace CPU_Host
{
    using vm.component;

    public class HostContainer
    {
        public static readonly HostContainer Instance = new HostContainer();

        public Bus bus { get; set; } = new Bus();
    }
}