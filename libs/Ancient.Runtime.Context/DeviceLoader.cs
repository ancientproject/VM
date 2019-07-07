namespace ancient.runtime.context
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class DeviceLoader
    {
        public static DeviceImageLoadContext Context { get; internal set; }
        public static event Action<string> OnTrace;

        public static void Grub(Action<IDevice> hook, params string[] additionalImage) => Grub(hook, null, additionalImage);
        public static void Grub(Action<IDevice> hook, Action<string> trace, params string[] additionalImage)
        {
            if(Context is null)
                Context = new DeviceImageLoadContext(trace);

            var asmList = additionalImage
                .Select(imageName => Context.LoadFromAssemblyName(new AssemblyName(imageName)))
                .ToList();

            var devList = asmList
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => !x.IsAbstract)
                .Where(x => typeof(IDevice).IsAssignableFrom(x))
                .Select(Activator.CreateInstance)
                .Select(x => (IDevice) x);
            foreach (var dev in devList) hook(dev);
        }

        private static void trace(string str) => OnTrace?.Invoke(str);
    }
}