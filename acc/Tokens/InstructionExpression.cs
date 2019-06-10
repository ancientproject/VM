﻿namespace flame.compiler.tokens
{
    using runtime;
    using Sprache;

    public class InstructionExpression : IInputToken
    {
        public Position InputPosition { get; set; }
        public Instruction Instruction { get; set; }

        public InstructionExpression(Instruction ins) => Instruction = ins;
    }
}