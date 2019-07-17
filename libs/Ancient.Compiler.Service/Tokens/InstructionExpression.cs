namespace ancient.compiler.tokens
{
    using runtime;
    using Sprache;

    public class InstructionExpression : IInputToken
    {
        public Position InputPosition { get; set; }
        public Instruction Instruction { get; set; }

        public InstructionExpression(Instruction ins) => Instruction = ins;
    }

    public class NullExpression : IInputToken
    {
        public Position InputPosition { get; set; }
    }
}