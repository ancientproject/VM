namespace rune.etc.registry
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using Flurl.Http;
    using Newtonsoft.Json;
    using Octokit;

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

        public async Task<bool> Exist(string id)
        {
            Console.Write($"{":thought_balloon:".Emoji()} GET '{owner}/{id}'...".Color(Color.DimGray));

            var repos = await client.Repository.GetAllForUser(owner);
            var result = repos.Any(x => x.Name == id);

            Console.WriteLine(result ? $".. OK".Color(Color.DimGray) : $".. 404".Color(Color.DimGray));
            return result;
        }

        public async Task<(Assembly assembly, byte[] raw)> Fetch(string id)
        {
            var repo = (await client.Repository.GetAllForUser(owner)).FirstOrDefault(x => x.Name == id);
            if (repo is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{id}' not found in '{owner}'".Color(Color.Orange));
                return default;
            }

            var content = await client.Repository.Content.GetAllContents(repo.Id);

            if (content is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{owner}/{id}' failed fetch files.".Color(Color.Orange));
                return default;
            }

            var box = content.FirstOrDefault(x => x.Name == "box.json");

            if (box is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] '{owner}/{id}' is not a {"package".Nier()}.".Color(Color.Orange));
                return default;
            }
            Console.Write($"{":thought_balloon:".Emoji()} PUT '{owner}/{id}'...".Color(Color.DimGray));
            var temp = await box.DownloadUrl.WithTimeout(10).GetStringAsync();

            var ancientBox = JsonConvert.DeserializeObject<AncientPackageBox>(temp);

            var file = content.FirstOrDefault(x => x.Name == ancientBox.files.First());
            Console.WriteLine($".. OK".Color(Color.DimGray));
            if (file is null)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} [github] {"package".Nier()} '{owner}/{id}' box.json bad.".Color(Color.Orange));
                return default;
            }
            var raw = CSharpCompile.Build(id, await file.DownloadUrl.WithTimeout(10).GetStringAsync());
            return (Assembly.Load(raw), raw);
        }
    }
}