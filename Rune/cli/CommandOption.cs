namespace rune.cli
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandOption
    {
        internal CommandOption(string template, CommandOptionType optionType)
        {
            Template = template;
            OptionType = optionType;
            Values = new List<string>();

            foreach (var part in Template.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                    LongName = part.Substring(2);
                else if (part.StartsWith("-"))
                {
                    var optName = part.Substring(1);
                    if (optName.Length == 1 && !IsEnglishLetter(optName[0]))
                        SymbolName = optName;
                    else
                        ShortName = optName;
                }
                else if (part.StartsWith("<") && part.EndsWith(">"))
                    ValueName = part.Substring(1, part.Length - 2);
                else if (optionType == CommandOptionType.MultipleValue && part.StartsWith("<") && part.EndsWith(">..."))
                    ValueName = part.Substring(1, part.Length - 5);
                else
                    throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
            }

            if (string.IsNullOrEmpty(LongName) && string.IsNullOrEmpty(ShortName) && string.IsNullOrEmpty(SymbolName))
                throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
        }

        public string Template { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string SymbolName { get; set; }
        public string ValueName { get; set; }
        public string Description { get; set; }
        public List<string> Values { get; }
        public bool? BoolValue { get; private set; }
        internal CommandOptionType OptionType { get; }

        public bool TryParse(string value)
        {
            switch (OptionType)
            {
                case CommandOptionType.MultipleValue:
                    Values.Add(value);
                    break;
                case CommandOptionType.SingleValue:
                    if (Values.Any())
                        return false;
                    Values.Add(value);
                    break;
                case CommandOptionType.BoolValue:
                    if (Values.Any())
                        return false;

                    if (value == null)
                    {
                        Values.Add(null);
                        BoolValue = true;
                    }
                    else
                    {
                        if (!bool.TryParse(value, out var boolValue))
                            return false;
                        Values.Add(value);
                        BoolValue = boolValue;
                    }
                    break;
                case CommandOptionType.NoValue:
                    if (value != null)
                        return false;
                    Values.Add("on");
                    break;
            }
            return true;
        }

        public bool HasValue() => Values.Any();
        public string Value() => HasValue() ? Values[0] : null;
        private bool IsEnglishLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
    internal enum CommandOptionType
    {
        MultipleValue,
        SingleValue,
        BoolValue,
        NoValue
    }
}