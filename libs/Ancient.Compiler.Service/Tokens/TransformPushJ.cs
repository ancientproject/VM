namespace ancient.compiler.tokens
{
    using System.Linq;
    using runtime;

    public class TransformPushJ : TransformationContext
    {
        public TransformPushJ(string value, byte cellDev, byte ActionDev)
        {
            Instructions = value.Select(x => new push_a(cellDev, ActionDev, x)).Cast<Instruction>().ToArray();
        }
    }
}