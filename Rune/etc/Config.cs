namespace rune.etc
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using IniFile;

    public static class Config
    {
        public static string GetRaw() 
            => Dirs.ConfigFile.ReadToEnd();

        public static T Get<T>(string section, string key, T @default)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
                throw new ContextMarshalException($"Cast 'string' to '{typeof(T).Name}' not supported.");
            var target = default(Section);
            if ((target = new Ini(Dirs.ConfigFile)[section]) is null)
                return @default;
            var value = default(PropertyValue);
            if ((value = target[key]) == default)
                return @default;
            if (value.IsEmpty())
                return @default;
            return (T)converter.ConvertFrom(value.ToString());
        }

        public static void Set<T>(string section, string key, T value)
        {
            if(string.IsNullOrEmpty(section))
                throw new ArgumentNullException(nameof(section));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if(value is null)
                throw new ArgumentNullException(nameof(value));
            if(!TypeDescriptor.GetConverter(typeof(T)).CanConvertTo(typeof(string)))
                throw new ContextMarshalException($"Cast '{typeof(T).Name}' to 'string' not supported.");

            var c = new Ini(Dirs.ConfigFile);

            if (c[section] is null)
                c.Add(new Section(section)
                {
                    new Property(key, Convert.ToString(value, CultureInfo.InvariantCulture))
                });
            else
                c[section][key] = Convert.ToString(value, CultureInfo.InvariantCulture);


            c.SaveTo(Dirs.ConfigFile);
        }

        public static void Ensure()
        {
            Dirs.Ensure();
            if(Dirs.ConfigFile.Exists)
                return;
            var c = new Ini();
            c.SaveTo(Dirs.ConfigFile);
        }
    }
}