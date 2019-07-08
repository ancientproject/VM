namespace Ancient.ProjectSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class AncientProjectFile
    {
        public string name { get; set; }
        public string version { get; set; }
        /// <summary>
        /// <see cref="string"/> or <see cref="AncientAuthor"/>
        /// </summary>
        public object author { get; set; }

        public string extension { get; set; }

        public Dictionary<string, string> scripts = new Dictionary<string, string>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> deps { get; set; }

        internal static AncientProjectFile Open(FileInfo file) 
            => JsonConvert.DeserializeObject<AncientProjectFile>(File.ReadAllText(file.FullName));
    }

    public enum DepVersionKind
    {
        Fixed,
        Less,
        More
    }

    public class AncientProject
    {
        /// <summary>
        /// Name of project
        /// </summary>
        public string Name => project.name;
        /// <summary>
        /// Extension of output file
        /// </summary>
        public string Extension => project.extension;

        public Dictionary<string, string> deps => project.deps ?? new Dictionary<string, string>();

        public Dictionary<string, string> scripts => project.scripts ?? new Dictionary<string, string>();

        private readonly FileInfo projectRef;
        private readonly AncientProjectFile project;

        public AncientProject(FileInfo file)
        {
            projectRef = file;
            project = AncientProjectFile.Open(projectRef);
        }


        public AncientProject AddDep(string id, string version, DepVersionKind kind)
        {
            if (Exist(id))
            {
                DepsVersion(id, out var ver);

                if (ver.IsCurrent(version) || ver.IsDowngrade(version))
                    return this;
                project.deps[id] = DepVersion.From(version, true).WithKind(kind).ToString();
            }
            else
            {
                if(project.deps is null) project.deps = new Dictionary<string, string>();
                project.deps.Add(id, DepVersion.From(version, true).WithKind(kind).ToString());
            }
            return Flush();
        }

        public AncientProject RemDep(string id)
        {
            if (Exist(id) && project.deps.Remove(id))
                return Flush();
            return this;
        }

        public AncientProject DepsVersion(string id, out DepVersion version)
        {
            if (Exist(id))
            {
                version = DepVersion.From(project.deps[id]);
                return this;
            }
            version = null;
            return this;
        }

        public bool Exist(string id)
        {
            if (project.deps is null)
                return false;
            return project.deps.ContainsKey(id);
        }

        private AncientProject Flush()
        {
            File.WriteAllText(projectRef.FullName, JsonConvert.SerializeObject(project));
            return this;
        }


        public static AncientProject FromLocal()
        {
            var dir = Directory.GetCurrentDirectory();
            var info = new DirectoryInfo(dir);
            var json = info.GetFiles("*.rune.json");
            return new AncientProject(json.First());
        }
    }

    public class DepVersion
    {
        public DepVersionKind kind { get; private set; }
        public string version { get; private set; }

        private DepVersion(string _ver, bool withoutKind)
        {
            if(!withoutKind)
                kind = getKind(ref _ver);
            version = _ver;
        }

        public static DepVersion From(string version, bool withoutKind = false) => new DepVersion(version, withoutKind);

        public bool IsUpgrade(string anotherVersion) 
            => new Version(version) < new Version(anotherVersion);
        public bool IsDowngrade(string anotherVersion) 
            => new Version(version) > new Version(anotherVersion);
        public bool IsCurrent(string anotherVersion) 
            => new Version(version) == new Version(anotherVersion);

        private DepVersionKind getKind(ref string ver)
        {
            var controlChar = ver[0];
            ver = ver.Remove(0, 1);
            switch (controlChar)
            {
                case '>':
                    return DepVersionKind.More;
                case '<':
                    return DepVersionKind.Less;
                case '^':
                    return DepVersionKind.Fixed;
            }
            throw new NotSupportedException($"'{ver}' is not valid dep version kind.");
        }

        public DepVersion WithKind(DepVersionKind verKind)
        {
            kind = verKind;
            return this;
        }

        private char getKindChar(DepVersionKind verKind)
        {
            switch (verKind)
            {
                case DepVersionKind.More:
                    return '>';
                case DepVersionKind.Less:
                    return '<';
                case DepVersionKind.Fixed:
                    return '^';
            }
            throw new NotSupportedException($"Unknown dep version kind.");
        }

        public override string ToString() => $"{getKindChar(kind)}{version}";
    }


    public class AncientAuthor
    {
        public string name;
        public string email;
    }
}
