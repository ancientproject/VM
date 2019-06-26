namespace Rune.cli
{
    using System;

    internal class CommandParsingException : Exception
    {
        public CommandParsingException(CommandLineApplication command, string message)
            : base(message) => Command = command;

        public CommandLineApplication Command { get; }
    }
}