namespace flame.compiler.emit
{
    using System.Collections.Generic;
    using tokens;

    public class DebugSymbolsSegment : IChainSegment<string>
    {
        public string Transform(IReadOnlyCollection<IInputToken> tokens)
        {
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case InstructionExpression i:
                        break;
                    case TransformationContext t:
                        break;
                }
            }

            return null;
        }
    }
}