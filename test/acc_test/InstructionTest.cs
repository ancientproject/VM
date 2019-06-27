namespace ancient.runtime.compiler.test
{
    using System.Linq;
    using ancient.compiler;
    using ancient.compiler.tokens;
    using runtime;
    using Sprache;
    using Xunit;

    public class InstructionTest
    {
        [Theory]
        [InlineData(".n_value @float_t(14.56)")]
        public void FloatParseTest(string code)
        {
            var result = new FlameAssemblerSyntax().NValue.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is n_value i)
            {
                Assert.Equal(14.56f, i._data);
            }
        }
        [Fact()]
        public void TransformationTest()
        {
            const string str = 
                             @"#{ 
                                    ~label 'test' 0x2
                                    ~label 'benis' 0x5
                                }
                                .push_a &(![~test]) &(![~beniz]) <| $(![~beniz])";
            var result = new FlameTransformerSyntax().ManyEvolver
                .End().Parse(str);

            Assert.Equal(2, result.Length);
        }
        [Fact]
        public void ShiftTest()
        {
            var shifter = ShiftFactory.Create(28);
            Assert.Equal(28, shifter.Shift());
            Assert.Equal(24, shifter.Shift());
            Assert.Equal(20, shifter.Shift());
            Assert.Equal(16, shifter.Shift());
            Assert.Equal(12, shifter.Shift());
            Assert.Equal(8, shifter.Shift());
            Assert.Equal(4, shifter.Shift());
            Assert.Equal(0, shifter.Shift());
            Assert.Equal(0, shifter.Shift());
        }

        [Theory]
        [InlineData(".push_j &(0x0) &(0xC) <| @string_t(\"test\")")]
        public void PushJ(string code)
        {
            var result = Host.Evolve(code).Split('\n');
            Assert.Equal(".push_a &(0x0) &(0xC) <| $(0x74)", result.First());
            Assert.Equal(".push_a &(0x0) &(0xC) <| $(0x74)", result.Last());
        }
        [Theory]
        [InlineData(".push_a &(0x0) &(0xC) <| $(0xFF)")]
        [InlineData(".push_a &(0x0) &(0xC) <| @char_t('ÿ')")]
        public void PushA(string code)
        {
            var result = new FlameAssemblerSyntax().PushA.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is push_a i)
            {
                Assert.Equal(InsID.push_a, i.ID);
                Assert.Equal(0x0, i._addressBus);
                Assert.Equal(0xC, i._addressDev);
                Assert.Equal(0xFF, i._value);
                Assert.Equal(0xF0C00FFC, (uint)i.Assembly());
            }
        }
        [Theory]
        [InlineData(".loadi &(0x0) <| $(0xF)")]
        public void LoadI(string code)
        {
            var result = new FlameAssemblerSyntax().LoadI.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is loadi i)
            {
                Assert.Equal(InsID.loadi, i.ID);
                Assert.Equal(0x0, i._index);
                Assert.Equal(0xF, i._value);
            }
        }
        [Theory]
        [InlineData(".jump_t &(0x0)")]
        public void JumpT(string code)
        {
            var result = new FlameAssemblerSyntax().JumpT.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is jump_t i)
            {
                Assert.Equal(InsID.jump_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
        }
        [Theory]
        [InlineData(".ref_t &(0x0)")]
        public void RefT(string code)
        {
            var result = new FlameAssemblerSyntax().RefT.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is ref_t i)
            {
                Assert.Equal(InsID.ref_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
        }
        [Theory]
        [InlineData(".swap &(0x0) &(0xF)")]
        public void Swap(string code)
        {
            var result = new FlameAssemblerSyntax().SwapToken.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is swap i)
            {
                Assert.Equal(InsID.swap, i.ID);
                Assert.Equal(0x0, i._index1);
                Assert.Equal(0xF, i._index2);
            }
        }
        [Theory]
        [InlineData(".halt")]
        public void Halt(string code)
        {
            var result = new FlameAssemblerSyntax().ByIIDToken(InsID.halt).End().Parse(code);

            if (result.Instruction is halt i)
                Assert.Equal(InsID.halt, i.ID);
        }
        [Theory]
        [InlineData(".warm")]
        public void Warm(string code)
        {
            var result = new FlameAssemblerSyntax().ByIIDToken(InsID.warm).End().Parse(code);

            if (result.Instruction is warm i)
                Assert.Equal(InsID.warm, i.ID);
        }

        [Theory]
        [InlineData(".jump_t &(0xF)")]
        [InlineData(".jump_e &(0xF) -~ &(0x9) &(0x9)")]
        [InlineData(".jump_g &(0xF) -~ &(0x9) &(0x9)")]
        [InlineData(".jump_u &(0xF) -~ &(0x9) &(0x9)")]
        [InlineData(".jump_y &(0xF) -~ &(0x9) &(0x9)")]
        public void JumperTest(string code)
        {
            var result = new FlameAssemblerSyntax().ManyParser.End().Parse(code).First();

            if (result is InstructionExpression i)
            {
                Assert.Contains(i.Instruction.GetType().Name, code);
                if(i.Instruction is jump_t t)
                    Assert.Equal(0x8F000F00, (uint)t.Assembly());
                if(i.Instruction is jump_e e)
                    Assert.Equal(0x8F990F10, (uint)e.Assembly());
                if(i.Instruction is jump_g g)
                    Assert.Equal(0x8F990F20, (uint)g.Assembly());
                if(i.Instruction is jump_u u)
                    Assert.Equal(0x8F990F30, (uint)u.Assembly());
                if(i.Instruction is jump_y y)
                    Assert.Equal(0x8F990F40, (uint)y.Assembly());
            }
        }
    }
}
