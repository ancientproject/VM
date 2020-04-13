namespace rune.etc.registry
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Ancient.ProjectSystem;
    using Flurl.Http;

    public class RunicRegistry : IRegistry
    {

        public static string Endpoint = "https://api.ruler.runic.cloud";
        public static string Name = "registry.runic.cloud";

        public async Task<(Assembly, byte[])> Fetch(string id)
        {
            // todo: logic for cache
            //var cacheFolder = EnsureFolders();
            //if(Directory.Exists(Path.Combine(cacheFolder, $"{id}")))

            if (!await Exist(id))
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} {"package".Nier()} '{id}' not found in '{Name}'".Color(Color.Orange));
                return default;
            }

            Console.Write($"{":thought_balloon:".Emoji()} FETCH '{Name}/{id}'...".Color(Color.DimGray));

            var req = Endpoint
                .WithTimeout(2)
                .AllowAnyHttpStatus()
                .AppendPathSegment($"/api/registry/@/{id}")
                .GetAsync().Result;

            if (!req.IsSuccessStatusCode)
            {
                Console.WriteLine($"{":thought_balloon:".Emoji()} {"package".Nier()} '{Name}/{id}' failed fetch files.".Color(Color.Orange));
                return default;
            }

            Console.WriteLine($".. OK".Color(Color.DimGray));

            var memory = new MemoryStream();
            req.Content.CopyToAsync(memory).Wait();

            var result = await RunePackage.Unwrap(memory);

            return default;
        }

        public async Task<bool> Exist(string id)
        {
            Console.Write($"{":thought_balloon:".Emoji()} PROPFIND '{Name}/{id}'...".Color(Color.DimGray));

            var request = await Endpoint
                .WithTimeout(2)
                .AllowAnyHttpStatus()
                .AppendPathSegment($"/api/registry/@/{id}")
                .SendAsync(new HttpMethod("PROPFIND"));

            if (request.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(".. OK".Color(Color.DimGray));
                return true;
            }
            Console.WriteLine($".. {request.StatusCode}".Color(Color.DimGray));
            return false;
        }

        private string EnsureFolders()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Combine(path, ".rune/packages");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}