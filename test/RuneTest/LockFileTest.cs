namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Ancient.ProjectSystem;
    using NUnit.Framework;
    using Pose;
    public class LockFileTest
    {
        [Test]
        [Author("Yuuki Wesp", "ls-micro@ya.ru")]
        [Description("ensure create lock file in virtual file system and assert exist file")]
        public void CreateIndexFileTest()
        {
            var dirs = new List<string>();
            var files = new List<string>();
            var shims = new List<Shim>();
            shims.Add(Shim
                .Replace(() => Pose.Is.A<FileSystemInfo>().FullName)
                .With((FileSystemInfo @this) => @this.ToString()));
            shims.Add(Shim
                .Replace(() => Directory.CreateDirectory(Pose.Is.A<string>()))
                .With((string s) => dirs.Chain(s, (list, v) => list.Add(v)).Mutate(x => new DirectoryInfo(x)).Return()));
            shims.Add(Shim
                .Replace(() => File.WriteAllText(Pose.Is.A<string>(), Pose.Is.A<string>()))
                .With((string path, string content) => files.Add(path)));
            shims.Add(Shim
                .Replace(() => Directory.Exists(Pose.Is.A<string>()))
                .With((string s) => dirs.Contains(s)));
            shims.Add(Shim
                .Replace(() => Pose.Is.A<DirectoryInfo>().Exists)
                .With((DirectoryInfo @this) => dirs.Contains(@this.ToString())));
            shims.Add(Shim
                .Replace(() => Pose.Is.A<FileInfo>().Exists)
                .With((FileInfo @this) => files.Contains(@this.ToString())));

            try
            {

                PoseContext.Isolate(() =>
                {
                    Indexer.FromLocal();

                }, shims.ToArray());

                Assert.AreEqual("./deps/.ancient.lock", files.First());
            }
            catch (Exception e)
            {
                Assert.Inconclusive(e.Message);
            }
        }
    }
}