namespace Ancient.ProjectSystem
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public class AncientProject
    {
        public string name { get; set; }
        public string version { get; set; }
        /// <summary>
        /// <see cref="string"/> or <see cref="AncientAuthor"/>
        /// </summary>
        public object author { get; set; }

        public Dictionary<string, string> scripts = new Dictionary<string, string>();


        public static AncientProject Open(FileInfo file)
        {
            return JsonConvert.DeserializeObject<AncientProject>(File.ReadAllText(file.FullName));
        }
    }


    public class AncientAuthor
    {
        public string name;
        public string email;
    }
}
