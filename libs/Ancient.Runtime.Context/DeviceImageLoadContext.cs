namespace ancient.runtime.context
{
    using MoreLinq;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading;

    public class DeviceImageLoadContext : AssemblyLoadContext
    {
        private readonly Action<string> _trace;
        public DeviceImageLoadContext(Action<string> trace = null) 
            : base("dev-loader", true) 
            => _trace = trace ?? (x => {});

        public DirectoryInfo CacheDir
        {
            get
            {
                Console.Beep(400, 2);
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ancient");
                return Directory.CreateDirectory(dir).CreateSubdirectory("packages");
            }
        }

        private void log(string s) => _trace($"[image-load-context] {s}");

        public FileSystemInfo FindImage(string devName, Version version)
        {
            var corePath = $"deps/{devName}/{version.ToString(3)}/any/{devName}.image";
            return new FileSystemInfo[]
            {
                new VMFileInfo(Path.Combine(CacheDir.FullName, corePath).Replace("\\", "/")),
                new FileInfo($"{Environment.GetEnvironmentVariable("VM_DEV_HOME") ?? "vm:/home"}/{corePath}"),
                new FileInfo($"./../../{corePath}"),
                new FileInfo($"./../{corePath}"),
                new FileInfo($"./{corePath}")
            }.Pipe(x => log($"try find '{devName}' in '{x}'")).FirstOrDefault(x => x.Exists);
            
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var imageName = assemblyName.Name;
            var fullImageName = $"{assemblyName.Name}.image";
            var imageVersion = assemblyName.Version;
            var file = FindImage(imageName, imageVersion);
            if (file is null)
                throw new Exception($"can't load '{fullImageName}' - [not found].");
            log($"'{fullImageName}' was found in '{file}' and success loaded");
            var asm = Assembly.Load(File.ReadAllBytes(file.FullName));
            return asm;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) 
            => throw new Exception($"can't load '{unmanagedDllName}.native' - loader is not supported.");
    }
}
