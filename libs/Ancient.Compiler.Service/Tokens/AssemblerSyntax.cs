namespace ancient.compiler.tokens
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using runtime;
    using runtime.emit.sys;
    using Sprache;
    using Module = runtime.emit.sys.Module;

    public partial class AssemblerSyntax
    {
        internal static readonly Dictionary<string, OperatorKind> Operators = new Dictionary<string, OperatorKind>
        {
            ["."] = OperatorKind.Dot,
            ["|>"] = OperatorKind.PipeLeft,
            ["<|"] = OperatorKind.PipeRight,
            ["&"] = OperatorKind.Ref,
            ["$"] = OperatorKind.Value,
            ["^"] = OperatorKind.AltRef,
            ["("] = OperatorKind.OpenParen,
            [")"] = OperatorKind.CloseParen,
            ["~-"] = OperatorKind.When
        };

        internal static readonly Dictionary<OperatorKind, string> OperatorsReversed =
            Operators.Reverse().ToDictionary(x => x.Value, x => x.Key);

        public virtual Parser<IInputToken> Parser => CommentToken
            // base instruction token
            .Or(SwapToken)
            .Or(RefT)
            .Or(PushA).Or(PushD).Or(PushX)
            .Or(LoadI)
            .Or(LoadI_X)
            .Or(LoadI_S)
            .Or(RFD)
            //etc
            .Or(StageN)
            .Or(NValue)
            .Or(Raw)
            .Or(Dif_f)
            .Or(Dif_t)
            .Or(CKFT)
            .Or(Dup)
            .Or(Call_I)
            .Or(StaticExternCall)
            .Or(LPSTR)
            // logical
            .Or(MultipleSignatureToken(IID.or))
            .Or(MultipleSignatureToken(IID.xor))
            .Or(MultipleSignatureToken(IID.and))
            .Or(MultipleSignatureToken(IID.ceq))
            .Or(MultipleSignatureToken(IID.neq))
            // jumps
            .Or(JumpT)
            .Or(JumpAt(IID.jump_e))
            .Or(JumpAt(IID.jump_g))
            .Or(JumpAt(IID.jump_u))
            .Or(JumpAt(IID.jump_y))
            .Or(JumpP)
            .Or(JumpX)
            // empty instruction token
            .Or(ByIIDToken(IID.halt))
            .Or(ByIIDToken(IID.warm))
            // break instruction
            .Or(ByIIDToken(IID.brk_a))
            .Or(ByIIDToken(IID.brk_n))
            .Or(ByIIDToken(IID.brk_s))
            // etc
            .Or(ByIIDToken(IID.nop))
            .Or(ByIIDToken(IID.prune))
            .Or(Unlock)
            // math instruction token
            .Or(MultipleSignatureToken(IID.add))
            .Or(MultipleSignatureToken(IID.mul))
            .Or(MultipleSignatureToken(IID.sub))
            .Or(MultipleSignatureToken(IID.div))
            .Or(MultipleSignatureToken(IID.pow))
            .Or(MultipleSignatureToken(IID.sqrt))

            .Or(MultipleSignatureToken(IID.min))
            .Or(MultipleSignatureToken(IID.max))
            .Or(MultipleSignatureToken(IID.atan2))
            // advanced math
            .Or(MultipleSignatureToken(IID.abs)).Or(MultipleSignatureToken(IID.acos)).Or(MultipleSignatureToken(IID.atanh))
            .Or(MultipleSignatureToken(IID.atan))
            .Or(MultipleSignatureToken(IID.acosh)).Or(MultipleSignatureToken(IID.asin))
            .Or(MultipleSignatureToken(IID.asinh)).Or(MultipleSignatureToken(IID.cbrt)).Or(MultipleSignatureToken(IID.cell))
            .Or(MultipleSignatureToken(IID.cosh)).Or(MultipleSignatureToken(IID.cos)).Or(MultipleSignatureToken(IID.flr))
            .Or(MultipleSignatureToken(IID.exp)).Or(MultipleSignatureToken(IID.log10)).Or(MultipleSignatureToken(IID.log))
            .Or(MultipleSignatureToken(IID.tanh))
            .Or(MultipleSignatureToken(IID.tan)).Or(MultipleSignatureToken(IID.trc))
            .Or(MultipleSignatureToken(IID.biti)).Or(MultipleSignatureToken(IID.bitd))
            .Or(MultipleSignatureToken(IID.sinh)).Or(MultipleSignatureToken(IID.sin))

            .Or(AdvMathInstruction(IID.inv))
            .Or(AdvMathInstruction(IID.inc))
            .Or(AdvMathInstruction(IID.dec))

            

            // transformators
            .Or(Locals.Return(new NullExpression()))
            .Or(Group(Label).Return(new NullExpression()))
        ;

        

        #region etc

        protected internal virtual Parser<T> Wrap<T, S>(Parser<T> el, Parser<S> wrapper) =>
            from _1 in wrapper
            from result in el
            from _2 in wrapper
            select result;

        #endregion



       

        #region Instructuions token

        #region Advanced Instruction token

        public virtual Parser<IInputToken> MultipleSignatureToken(IID id) => (
                from dword in InstructionToken(id)
                from body in
                    from bodyToken in MultipleSyntaxCells(Instruction.GetArgumentCountBy(id) - 1)
                    select bodyToken.GetValueOrDefault((null, new byte[0]))
                select new InstructionExpression(Instruction.Summon(id, new object[]{ body.result }.Concat(body.args.Cast<object>().ToArray()).ToArray())))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        private static (byte? result, byte[] args)? DecomposeSignature(
            (RefExpression result, RefExpression c1, RefExpression c2)? o1,
            (RefExpression c1, RefExpression c2)? o2,
            RefExpression o3
        )
        {
            if (o1 is { } r1)
                return (r1.result.Cell, new []{ r1.c1.Cell, r1.c2.Cell });
            if (o2 is { } r2)
                return (null, new []{ r2.c1.Cell, r2.c2.Cell });
            if (o3 is { })
                return (o3.Cell, new byte[0]);
            return null;
        }
        private static (byte? result, byte[] args)? DecomposeSignature(
            (RefExpression result, RefExpression c1)? o1,
            (RefExpression c1, RefExpression c2)? o2,
            RefExpression o3
        )
        {
            if (o1 is { } r1)
                return (r1.result.Cell, new[] { r1.c1.Cell });
            if (o2 is { } r2)
                return (null, new[] { r2.c1.Cell, r2.c2.Cell });
            if (o3 is { })
                return (o3.Cell, new byte[0]);
            return null;
        }

        public virtual Parser<(byte? result, byte[] args)?> MultipleSyntaxCells(int argumentCount)
        {
            if(argumentCount == 2)
            return 
                from n1 in PairArgumentWithPipe.Optional()
                from n2 in (
                    from cell1 in RefToken
                    from cell2 in RefToken
                    select (cell1, cell2)).Optional()
                from n3 in RefToken.Optional()
                select DecomposeSignature(n1.Unwrap(), n2.Unwrap(), n3.GetOrDefault());
            return (

                from n1 in ArgumentWithPipe.Except(PairArgumentWithPipe).Optional()
                from n3 in RefToken.Optional()
                let result = DecomposeSignature(
                    n1.Unwrap(),
                    null,
                    n3.GetOrDefault())
                select result
            );
        }

        private Parser<(RefExpression, RefExpression)> ArgumentPair => 
            from cell1 in RefToken
            from cell2 in RefToken
            select (cell1, cell2);


        private Parser<(RefExpression, RefExpression, RefExpression)> PairArgumentWithPipe =>
            (
                from resultCell in RefToken
                from _ in PipeRight
                from cellPair in 
                    from cell1 in RefToken
                    from cell2 in RefToken 
                    select (cell1, cell2)
                select (resultCell, cellPair.cell1, cellPair.cell2)
            )
            .Token();
        private Parser<(RefExpression, RefExpression)> ArgumentWithPipe =>
            (
                from resultCell in RefToken
                from _ in PipeRight
                from cellPair in
                    from cell1 in RefToken
                    from cell2 in RefToken.Not("instruction available has no more cells")
                    select (cell1, cell2)
                select (resultCell, cellPair.cell1)
            )
            .Token();

        #endregion

        public virtual Parser<IInputToken> Dif_f =>
            (from dword in InstructionToken(IID.dif_f)
             from cell in RefToken
             from count in ValueToken
             select new InstructionExpression(new dif_f(cell.Cell, (byte)count.Value)))
            .Token()
            .WithPosition()
            .Named("dif_f expression");
        public virtual Parser<IInputToken> Dif_t =>
            (from dword in InstructionToken(IID.dif_t)
                from cell in RefToken
                from count in ValueToken
                select new InstructionExpression(new dif_t(cell.Cell, (byte)count.Value)))
            .Token()
            .WithPosition()
            .Named("dif_t expression");

        public virtual Parser<IInputToken> Raw =>
            (from dword in InstructionToken(IID.raw)
                from space1 in Parse.WhiteSpace.Optional()
                from val1 in HexToken
                select new InstructionExpression(new raw(ulong.Parse(val1, NumberStyles.AllowHexSpecifier))))
            .Token()
            .WithPosition()
            .Named("raw expression");

        public virtual Parser<IInputToken> Call_I =>
            (from dword in InstructionToken(IID.call_i)
                from sign in SignatureToken
                select new InstructionExpression(new call_i(Module.CompositeIndex(sign))))
            .Token()
            .WithPosition()
            .Named("call_inner expression");
        public virtual Parser<IInputToken> StaticExternCall =>
            (from dword in InstructionToken(IID.__static_extern_call)
                from sign in SignatureToken
                select new InstructionExpression(new __static_extern_call(Module.CompositeIndex(sign))))
            .Token()
            .WithPosition()
            .Named("__static_extern_call expression");

        public virtual Parser<IInputToken> LPSTR =>
            (from dword in InstructionToken(IID.lpstr)
                from sign in SignatureToken
                select new InstructionExpression(new lpstr(sign.Replace("\"", ""))))
            .Token()
            .WithPosition()
            .Named("lower point string expression");

        public virtual Parser<IInputToken> StageN =>
            (from dword in InstructionToken(IID.orb)
                from val1 in RefToken
                select new InstructionExpression(new orb(val1.Cell)))
            .Token()
            .WithPosition()
            .Named("orb expression");

        public virtual Parser<IInputToken> CKFT =>
            (from dword in InstructionToken(IID.ckft)
                from val1 in RefToken
                select new InstructionExpression(new ckft(val1.Cell)))
            .Token()
            .WithPosition()
            .Named("ckft expression");

        public virtual Parser<IInputToken> Dup =>
            (from dword in InstructionToken(IID.dup)
                from val1 in RefToken
                from val2 in RefToken
                select new InstructionExpression(new dup(val1.Cell, val2.Cell)))
            .Token()
            .WithPosition()
            .Named("dup expression");
        

        public virtual Parser<IInputToken> NValue =>
            (from dword in InstructionToken(IID.val)
                from val1 in CastFloat
                select new InstructionExpression(new val(val1)))
            .Token()
            .WithPosition()
            .Named("val expression");


        public virtual Parser<IInputToken> LoadI =>
            (from dword in InstructionToken(IID.ldi)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new ldi(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("ldi expression");
        public virtual Parser<IInputToken> LoadI_X =>
            (from dword in InstructionToken(IID.ldx)
                from cell1 in RefToken
                from pipe in PipeRight
                from val1 in ValueToken
                select new InstructionExpression(new ldx(cell1.Cell, val1.Value)))
            .Token()
            .WithPosition()
            .Named("ldx expression");

        public virtual Parser<IInputToken> LoadI_S =>
            (from dword in InstructionToken(IID.pull)
                from cell1 in RefToken
                select new InstructionExpression(new pull(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("pull expression");
        public virtual Parser<IInputToken> JumpT =>
            (from dword in InstructionToken(IID.jump_t)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                select new InstructionExpression(new jump_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");
        public virtual Parser<IInputToken> JumpX =>
            (from dword in InstructionToken(IID.jump_x)
             from cell0 in RefToken
             from _ in When
             from cell1 in RefToken
             select new InstructionExpression(new jump_x(cell0.Cell, cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_x expression");

        public virtual Parser<IInputToken> JumpP =>
            (from dword in InstructionToken(IID.jump_p)
                from cell0 in RefToken
                select new InstructionExpression(new jump_p(cell0.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_p expression");
        public virtual Parser<IInputToken> JumpAt(IID id) =>
            (from dword in InstructionToken(id)
                from space1 in Parse.WhiteSpace.Optional()
                from cell0 in RefToken
                from _ in When
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("jump_t expression");
        public virtual Parser<IInputToken> SwapToken =>
            (from dword in InstructionToken(IID.swap)
                from space1 in Parse.WhiteSpace.Optional()
                from cell1 in RefToken
                from space2 in Parse.WhiteSpace.Optional()
                from cell2 in RefToken
                select new InstructionExpression(new swap(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("swap expression");
        public virtual Parser<IInputToken> RFD =>
            (from dword in InstructionToken(IID.rfd)
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(new rfd(cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named("mva expression");
        public virtual Parser<IInputToken> PushA =>
            (from dword in InstructionToken(IID.mva)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeRight
                from cell3 in ValueToken.Or(CastCharToken.Select(x => new ValueExpression($"{x:x}")))
             select new InstructionExpression(new mva(cell1.Cell, cell2.Cell, cell3.Value)))
            .Token()
            .WithPosition()
            .Named("mva expression");
        public virtual Parser<IInputToken> PushD =>
            (from dword in InstructionToken(IID.mvd)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new mvd(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("mvd expression");
        public virtual Parser<IInputToken> PushX =>
            (from dword in InstructionToken(IID.mvx)
                from cell1 in RefToken
                from cell2 in RefToken
                from op2 in PipeLeft
                from cell3 in RefToken
                select new InstructionExpression(new mvx(cell1.Cell, cell2.Cell, cell3.Cell)))
            .Token()
            .WithPosition()
            .Named("mvx expression");
        public virtual Parser<IInputToken> RefT => (
                from dword in InstructionToken(IID.ref_t)
                from cell1 in RefToken
                select new InstructionExpression(new ref_t(cell1.Cell)))
            .Token()
            .WithPosition()
            .Named("ref_t expression");

        public virtual Parser<IInputToken> Unlock => (
                from dword in InstructionToken(IID.unlock)
                from cell1 in RefToken
                from type in TypeToken
                select new InstructionExpression(new unlock(cell1.Cell, type)))
            .Token()
            .WithPosition()
            .Named("unlock expression");

        public virtual Parser<IInputToken> MathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                from cell2 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell, cell2.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        public virtual Parser<IInputToken> AdvMathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");
        public virtual Parser<IInputToken> Adv2MathInstruction(IID id) => (
                from dword in InstructionToken(id)
                from cell0 in RefToken
                from cell1 in RefToken
                select new InstructionExpression(Instruction.Summon(id, cell0.Cell, cell1.Cell)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

        #endregion
        public virtual Parser<string> ProcToken(string name) =>
            (from dot in Parse.Char('~')
                from ident in Parse.String(name).Text()
                select ident)
            .Token()
            .Named("~ident");
        public virtual Parser<string> InstructionToken(IID instruction) =>
            (from dot in Parse.Char('.')
                from ident in Parse.String(instruction.ToString().Replace("_", ".")).Text()
                select ident).Token().Named($"{instruction} expression");
        public virtual Parser<InstructionExpression> ByIIDToken(IID id) =>
            (from dword in InstructionToken(id)
                select new InstructionExpression(Instruction.Summon(id)))
            .Token()
            .WithPosition()
            .Named($"{id} expression");

    }
    
    public class CommentToken : IInputToken
    {
        public readonly string _comment;
        public Position InputPosition { get; set; }
        public CommentToken(string comment) => _comment = comment;
    }
}