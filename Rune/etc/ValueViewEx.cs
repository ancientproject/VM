namespace Rune.cli
{
    using DustInTheWind.ConsoleTools.InputControls;

    public static class ValueViewEx
    {
        public static ValueView<T> WithDefault<T>(this ValueView<T> @this, T def)
        {
            @this.AutocompleteDefaultValue = false;
            @this.AcceptDefaultValue = true;
            @this.DefaultValue = def;
            return @this;
        }
    }
}