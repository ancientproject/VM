namespace flame.runtime.compiler.test
{
    using flame.compiler.tokens;
    using runtime;
    using Sprache;
    using Xunit;

    public class InstructionTest
    {
        [Theory]
        [InlineData(".push_j &(0x0) &(0xC) <| @string_t(\"test\")")]
        public void PushJ(string code)
        {
            var result = SyntaxStorage.PushJ.End().Parse(code);

            if (result is TransformationContext i)
            {
                Assert.Equal(4, i.Instructions.Length);
            }
            else Assert.True(false);
        }

        [Theory]
        [InlineData(".push_a &(0x0) &(0xC) |> $(0xFF)")]
        [InlineData(".push_a &(0x0) &(0xC) |> @char_t('ÿ')")]
        public void PushA(string code)
        {
            var result = SyntaxStorage.PushA.End().Parse(code) as InstructionExpression;

            if (result.Instruction is push_a i)
            {
                Assert.Equal(InsID.push_a, i.ID);
                Assert.Equal(0x0, i._addressBus);
                Assert.Equal(0xC, i._addressDev);
                Assert.Equal(0xFF, i._value);
            }
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".loadi &(0x0) <| $(0xF)")]
        public void LoadI(string code)
        {
            var result = SyntaxStorage.LoadI.End().Parse(code) as InstructionExpression;

            if (result.Instruction is loadi i)
            {
                Assert.Equal(InsID.loadi, i.ID);
                Assert.Equal(0x0, i._index);
                Assert.Equal(0xF, i._value);
            }
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".jump_t &(0x0)")]
        public void JumpT(string code)
        {
            var result = SyntaxStorage.JumpT.End().Parse(code) as InstructionExpression;

            if (result.Instruction is jump_t i)
            {
                Assert.Equal(InsID.jump_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".ref_t &(0x0)")]
        public void RefT(string code)
        {
            var result = SyntaxStorage.RefT.End().Parse(code) as InstructionExpression;

            if (result.Instruction is ref_t i)
            {
                Assert.Equal(InsID.ref_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".swap &(0x0) &(0xF)")]
        public void Swap(string code)
        {
            var result = SyntaxStorage.SwapToken.End().Parse(code) as InstructionExpression;

            if (result.Instruction is swap i)
            {
                Assert.Equal(InsID.swap, i.ID);
                Assert.Equal(0x0, i._index1);
                Assert.Equal(0xF, i._index2);
            }
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".halt")]
        public void Halt(string code)
        {
            var result = SyntaxStorage.ByIIDToken(InsID.halt).End().Parse(code);

            if (result.Instruction is halt i)
                Assert.Equal(InsID.halt, i.ID);
            else Assert.True(false);
        }
        [Theory]
        [InlineData(".warm")]
        public void Warm(string code)
        {
            var result = SyntaxStorage.ByIIDToken(InsID.warm).End().Parse(code);

            if (result.Instruction is warm i)
                Assert.Equal(InsID.warm, i.ID);
            else Assert.True(false);
        }
    }
}
