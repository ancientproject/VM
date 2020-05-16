namespace ancient.runtime.compiler.test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ancient.compiler;
    using ancient.compiler.tokens;
    using emit.sys;
    using runtime;
    using Sprache;
    using @unsafe;
    using Xunit;

    public class InstructionTest
    {

        public class IIDGenerator : IEnumerable<IID>, IEnumerable<object[]>
        {
            private readonly List<IID> data = new List<IID>
            {
                IID.abs, IID.acos,
                IID.asin, IID.cos,
                IID.sin, IID.sinh,
                IID.atan2, IID.add,
                IID.sub, IID.mul,
                IID.div, IID.tan,
                IID.tanh, IID.trc,
                IID.bitd, IID.biti,
                IID.log10, IID.log
            };
            public IEnumerator<IID> GetEnumerator() 
                => data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();
            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() 
                => data.Select(x => new object[]{x}).GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(IIDGenerator))]
        public void SingleSignatureTest(IID code)
        {
            var result = new AssemblerSyntax().Parser.End().Parse($".{code}");

            if (result is InstructionExpression exp)
            {
                Assert.NotNull(exp.Instruction);
                Assert.Contains(exp.Instruction.GetType().Name, $"{code}");
            }
        }
        [Theory]
        [ClassData(typeof(IIDGenerator))]
        public void WithResultCellTest(IID code)
        {
            var result = new AssemblerSyntax().Parser.End().Parse($".{code} &(0x0)");

            if (result is InstructionExpression exp)
            {
                Assert.NotNull(exp.Instruction);
                Assert.Contains(exp.Instruction.GetType().Name, $"{code}");
            }
        }
        [Theory]
        [ClassData(typeof(IIDGenerator))]
        public void WithPairCellTest(IID code)
        {
            var args = Instruction.GetArgumentCountBy(code);
            var result = default(IInputToken);
            var source = $".{code} &(0x0) &(0x0)";
            var parser = new AssemblerSyntax().Parser.End();
            if (args == 3)
                result = parser.Parse(source);
            else
            {
                Assert.Throws<ParseException>(() =>
                {
                    parser.Parse(source);
                });
                return;
            }

            if (result is InstructionExpression exp)
            {
                Assert.NotNull(exp.Instruction);
                Assert.Contains(exp.Instruction.GetType().Name, $"{code}");
            }
        }
        [Theory]
        [ClassData(typeof(IIDGenerator))]
        public void WithPairCellAndResultCellTest(IID code)
        {
            var args = Instruction.GetArgumentCountBy(code);
            var result = default(IInputToken);
            var source = $".{code} &(0x0) <| &(0x0) &(0x0)";
            var parser = new AssemblerSyntax().Parser.End();
            if (args == 3)
                result = parser.Parse(source);
            else
            {
                Assert.Throws<ParseException>(() =>
                {
                    parser.Parse(source);
                });
                return;
            }

            

            if (result is InstructionExpression exp)
            {
                Assert.NotNull(exp.Instruction);
                Assert.Contains(exp.Instruction.GetType().Name, $"{code}");
            }
        }
        [Theory]
        [InlineData("...static.extern.call !{sys->test()}")]
        public void CallStaticInternalFunctions(string code)
        {
            var result = new AssemblerSyntax().Parser.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is __static_extern_call i)
            {
                Assert.Equal($"{(ulong)0x40D51EC0C0:X}", $"{i.Assembly():X}");
            }
            else Assert.True(false, "Instruction is not '__static_extern_call'");
        }

        [Theory]
        [InlineData(".inv &(0x0)")]
        public void NegativeTest(string code)
        {
            var result = new AssemblerSyntax().Parser.End().Parse(code);

            if (result is InstructionExpression exp && exp.Instruction is inv i)
            {
                Assert.Equal($"{(ulong)0xB500000000:X}", $"{i.Assembly():X}");
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
        [InlineData(".ldi &(0x0) <| $(0xF)", 0x0, 0xf)]
        [InlineData(".ldi &(0x11) <| $(0xFA)", 0x11, 0xFA)]
        public void LoadI(string code, int cell, int value)
        {
            var result = new AssemblerSyntax().LoadI.End().Parse(code);

            if(result is InstructionExpression exp && exp.Instruction is ldi i)
            {
                Assert.Equal(IID.ldi, i.ID);
                Assert.Equal(cell, i._index);
                Assert.Equal(value, i._value);
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
