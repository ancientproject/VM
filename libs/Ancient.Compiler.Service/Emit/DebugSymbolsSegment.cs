namespace ancient.compiler.emit
{
    using System;
    using System.Collections.Generic;
    using tokens;

    [Obsolete]
    public class DebugSymbolsSegment : IChainSegment<string>
    {
        public string Transform(IReadOnlyCollection<IInputToken> tokens)
        {
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case InstructionExpression _:
                        break;
                    case TransformationContext _:
                        break;
                }
            }

            return null;
        }
    }
}