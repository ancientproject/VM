namespace flame.compiler.tokens
{
    using System.Linq;
    using runtime;

    public class TransformPushJ : TransformationContext
    {
        public TransformPushJ(string value, short cellDev, short ActionDev)
        {
            Instructions = value.Select(x => new push_a(cellDev, ActionDev, (short) x)).Cast<Instruction>().ToArray();
        }
    }
}