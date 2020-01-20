namespace rune.etc
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Ancient.ProjectSystem;
    using Flurl.Http;
    using Newtonsoft.Json;
    using Octokit;

    internal enum RegistryType
    {
        github,
        nuget,
        pkg
    }

    public class Registry
    {
        private readonly string _url;
        private readonly RegistryType _type;

        private Registry(string url, RegistryType type)
        {
            _url = url;
            _type = type;
        }

        public static IRegistry By(string uri)
        {
            var rex = new Regex(@"(?<type>github|nuget|pkg)\+(?<url>https\:[\/.\w]+)").Match(uri);
            var url = rex.Groups["url"].Value;
            switch (Enum.Parse<RegistryType>($"{rex.Groups["type"].Value}"))
            {
                case RegistryType.github:
                    return new GitHubOrgRegistry(url);

                default:
                    throw new NotSupportedException();
            }
        }
    }

    public class GitHubOrgRegistry : IRegistry
    {
        private readonly string _url;
        private readonly GitHubClient client;
        private string owner => new Regex(@"https\:\/\/github\.com\/(?<id>\w+)").Match(_url).Groups["id"].Value;

        public GitHubOrgRegistry(string url)
        {
            _url = url;
            client = new GitHubClient(new ProductHeaderValue("0xF6"));
        }

        public bool Exist(string id)
        {
            Console.Write($"{":thought_balloon:".Emoji()} GET '{owner}/{id}'...".Color(Color.DimGray));

            var result = client.Repository.GetAllForUser(owner).Result.FirstOrDefault(x => x.Name == id) != null;

            Console.WriteLine(result ? $".. OK".Color(Color.DimGray) : $".. 404".Color(Color.DimGray));
            return result;
        }

        public Assembly Put(string id, out byte[] assembly)
        {
            var repo = client.Repository.GetAllForUser(owner).Result.FirstOrDefault(x => x.Name == id);
            assembly = Array.Empty<byte>();
            if (repo is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{id}' not found in '{owner}'".Color(Color.Orange));
                return null;
            }

            var content = client.Repository.Content.GetAllContents(repo.Id).Result;

            if (content is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{owner}/{id}' failed fetch files.".Color(Color.Orange));
                return null;
            }

            var box = content.FirstOrDefault(x => x.Name == "box.json");

            if (box is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] '{owner}/{id}' is not a {"package".Nier()}.".Color(Color.Orange));
                return null;
            }
            Console.Write($"{":thought_balloon:".Emoji()} PUT '{owner}/{id}'...".Color(Color.DimGray));
            var temp = box.DownloadUrl.WithTimeout(10).GetStringAsync().Result;

            var ancientBox = JsonConvert.DeserializeObject<AncientPackageBox>(temp);

            var file = content.FirstOrDefault(x => x.Name == ancientBox.files.First());
            Console.WriteLine($".. OK".Color(Color.DimGray));
            if (file is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{owner}/{id}' box.json bad.".Color(Color.Orange));
                return null;
            }
            assembly = CSharpCompile.Build(id, file.DownloadUrl.WithTimeout(10).GetStringAsync().Result);
            return Assembly.Load(assembly);
        }
    }

    public interface IRegistry
    {
        bool Exist(string id);

        Assembly Put(string id, out byte[] assemblyBytes);
    }
}