namespace rune.cli
{
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandArgument
    {
        public CommandArgument() => Values = new List<string>();

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Values { get; }
        public bool MultipleValues { get; set; }
        public string Value => Values.FirstOrDefault();
    }
}