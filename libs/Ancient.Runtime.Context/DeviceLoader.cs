namespace ancient.runtime.context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Ancient.ProjectSystem;
    using Newtonsoft.Json;

    public static class DeviceLoader
    {
        public static DeviceImageLoadContext Context { get; internal set; }
        public static event Action<string> OnTrace;

        static DeviceLoader()
        {
            if(AppFlag.GetVariable("VM_TRACE"))
                OnTrace += Console.WriteLine;

            //AssemblyLoadContext.Default.Resolving += (context, name) => {
            //    var assemblyPath = $"./{name.Name}.dll";
            //    return context.LoadFromAssemblyPath(assemblyPath);
            //};
        }

        public static void Boot(Action<IDevice> hook)
        {
            if(!Directory.Exists("deps"))
            {
                trace($"[device-loader] directory '{new DirectoryInfo("deps").FullName}' not found");
                return;
            }
            if(!new FileInfo("./deps/ancient.lock").Exists)
            {
                trace($"[device-loader] lock file not found.");
                return;
            }
            var locks = new List<AncientLockFile>();
            try
            {
                locks = JsonConvert.DeserializeObject<AncientLockFile[]>(File.ReadAllText("./deps/ancient.lock")).ToList();
            }
            catch (Exception e)
            {
                trace($"[device-loader] {e}");
            }

            if (!locks.Any())
            {
                trace($"[device-loader] lock file is empty.");
                return;
            }

            Grub(hook, locks.Select(x => 
                    new AssemblyName($"{x.id}, Version={x.version}"))
                    .ToArray());
        }
        public static void Grub(Action<IDevice> hook, params AssemblyName[] additionalImage)
        {
            Context ??= new DeviceImageLoadContext(trace);
            var asmList = additionalImage
                .Select(imageName => Context.LoadFromAssemblyName(imageName))
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