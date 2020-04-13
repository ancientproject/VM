namespace rune.etc
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using registry;

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
                case RegistryType.pkg:
                    return new RunicRegistry();
                default:
                    throw new NotSupportedException();
            }
        }
    }
    public interface IRegistry
    {
        Task<bool> Exist(string id);
        Task<(Assembly assembly, byte[] raw)> Fetch(string id);
    }
}