namespace ancient.compiler.tokens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using runtime;
    using runtime.emit.sys;
    using Sprache;

    public class LocalsInitEvolver : IEvolveToken
    {
        public LocalsInitEvolver(IReadOnlyCollection<EvaluationSegment> seg)
        {
            if(seg.Count > 255)
                throw new Exception("seg more 255 len");

            var mem = new List<Instruction>();

            mem.Add(new locals((byte) seg.Count));
            mem.AddRange(seg.SelectMany(x => new []{x.OpCode.idx, x.OpCode.type}));

            this.Result = mem.Select(x => x.Assembly()).Select(x => $".raw 0x{x:X}").ToArray();
        }

        public string[] Result;

        public Position InputPosition { get; set; }
    }

    
}