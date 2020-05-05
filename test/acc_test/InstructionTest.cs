namespace ancient.runtime.compiler.test
{
    using System;
    using System.Linq;
    using ancient.compiler;
    using ancient.compiler.tokens;
    using emit.sys;
    using emit.@unsafe;
    using runtime;
    using Sprache;
    using @unsafe;
    using Xunit;

    public class InstructionTest
    {
        [Theory]
        [InlineData(".neg &(0x0)")]
        public void NegativeTest(string code)
        {
            var result = new AssemblerSyntax().Parser.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is neg i)
            {
                Assert.Equal((ulong)0xB700000000, i.Assembly());
            }
        }

        [Theory]
        [InlineData(".call.i !{sys->memory->barrier()}")]
        public void CallInnerTest(string code)
        {
            var result = new AssemblerSyntax().Call_I.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is call_i i)
            {
                Assert.Equal(Module.CompositeIndex("sys->memory->barrier()"), i._sign);
            }
        }


        [Theory]
        [InlineData(".val @float_t(14.56)")]
        [InlineData(".val @float_t(-14.56)")]
        public void FloatParseTest(string code)
        {
            var result = new AssemblerSyntax().NValue.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is val i)
                Assert.Equal(14.56f, MathF.Abs((float) i._data));
        }
        [Fact]
        public void TransformationTest()
        {
            const string str = 
                             @"#{ 
                                    ~label 'test' 0x2
                                    ~label 'benis' 0x5
                                }
                                .mva &(![~test]) &(![~beniz]) <| $(![~beniz])";
            var result = new TransformerSyntax().ManyEvolver
                .End().Parse(str);

            Assert.Equal(2, result.Length);
        }
        [Fact()]
        public void LocalsTransform()
        {
            const string str = 
@".locals init #(
   [0x0] u32,
   [0x1] u16
)

.locals init #(
   [0x0] u64,
   [0x1] u64
)
";
            var result = new TransformerSyntax().ManyEvolver
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
        [InlineData(".mvj &(0x0) &(0xC) <| @string_t(\"test\")")]
        public void PushJ(string code)
        {
            var result = Host.Evolve(code).Split('\n');
            Assert.Equal(".mva &(0x0) &(0xC) <| $(0x74)", result.First());
            Assert.Equal(".mva &(0x0) &(0xC) <| $(0x74)", result.Last());
        }
        [Theory]
        [InlineData(".mva &(0x0) &(0xC) <| $(0x66)")]
        [InlineData(".mva &(0x0) &(0xC) <| @char_t('f')")]
        public void PushA(string code)
        {
            var result = new AssemblerSyntax().PushA.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is mva i)
            {
                Assert.Equal(IID.mva, i.ID);
                Assert.Equal(0x0, i._addressBus);
                Assert.Equal(0xC, i._addressDev);
                Assert.Equal(0x66, i._value);
            }
        }
        [Theory]
        [InlineData(".ldi &(0x0) <| $(0xF)")]
        public void LoadI(string code)
        {
            var result = new AssemblerSyntax().LoadI.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is ldi i)
            {
                Assert.Equal(IID.ldi, i.ID);
                Assert.Equal(0x0, i._index);
                Assert.Equal(0xF, i._value);
            }
        }
        [Theory]
        [InlineData(".jump.t &(0x0)")]
        public void JumpT(string code)
        {
            var result = new AssemblerSyntax().JumpT.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is jump_t i)
            {
                Assert.Equal(IID.jump_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
        }
        [Theory]
        [InlineData(".ref.t &(0x0)")]
        public void RefT(string code)
        {
            var result = new AssemblerSyntax().RefT.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is ref_t i)
            {
                Assert.Equal(IID.ref_t, i.ID);
                Assert.Equal(0x0, i._cell);
            }
        }
        [Theory]
        [InlineData(".swap &(0x0) &(0xF)")]
        public void Swap(string code)
        {
            var result = new AssemblerSyntax().SwapToken.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is swap i)
            {
                Assert.Equal(IID.swap, i.ID);
                Assert.Equal(0x0, i._index1);
                Assert.Equal(0xF, i._index2);
            }
        }
        [Theory]
        [InlineData(".halt")]
        public void Halt(string code)
        {
            var result = new AssemblerSyntax().ByIIDToken(IID.halt).End().Parse(code);

            if (result.Instruction is halt i)
                Assert.Equal(IID.halt, i.ID);
        }
        [Theory]
        [InlineData(".lpstr !{\"test\"}", "test")]
        [InlineData(".lpstr !{\"big string test bla bla\"}", "big string test bla bla")]
        [InlineData(".lpstr !{\"number 1234567 test\"}", "number 1234567 test")]
        [InlineData(".lpstr !{\"symbols !@#$%^&*\"}", "symbols !@#$%^&*")]
        public void LpStringTest(string code, string text)
        {
            var result = new AssemblerSyntax().LPSTR.End().Parse(code);
            if (result is InstructionExpression exp && exp.Instruction is lpstr lp)
                Assert.Equal($"34{(uint) NativeString.GetHashCode(text):X8}", $"{lp.Assembly():X}");

        }

        [Theory]
        [InlineData(".jump.t &(0xF)")]
        [InlineData(".jump.e &(0xF) ~- &(0x9) &(0x9)")]
        [InlineData(".jump.g &(0xF) ~- &(0x9) &(0x9)")]
        [InlineData(".jump.u &(0xF) ~- &(0x9) &(0x9)")]
        [InlineData(".jump.y &(0xF) ~- &(0x9) &(0x9)")]
        public void JumperTest(string code)
        {
            var result = new AssemblerSyntax().ManyParser.End().Parse(code).First();

            if (result is InstructionExpression i)
            {
                if(i.Instruction is jump_t t)
                    Assert.Equal($"{0x8F000F000:X}", $"{t.Assembly():X}");
                if(i.Instruction is jump_e e)
                    Assert.Equal($"{0x8F990F100:X}",$"{e.Assembly():X}");
                if(i.Instruction is jump_g g)
                    Assert.Equal($"{0x8F990F200:X}", $"{g.Assembly():X}");
                if(i.Instruction is jump_u u)
                    Assert.Equal($"{0x8F990F300:X}", $"{u.Assembly():X}");
                if(i.Instruction is jump_y y)
                    Assert.Equal($"{0x8F990F400:X}", $"{y.Assembly():X}");
            }
        }
    }
}
