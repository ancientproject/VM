namespace flame.compiler.tokens
{
    using runtime;
    using Sprache;

    public class InstructionExpression : IInputToken
    {
        private readonly string _failedError;
        public Position InputPosition { get; set; }
        public Instruction Instruction { get; set; }

        public InstructionExpression(Instruction ins) => Instruction = ins;
        public InstructionExpression(string failedError) => _failedError = failedError;
    }
}