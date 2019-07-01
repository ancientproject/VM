namespace vm.component
{
    using System;

    public static class RangeEx
    {
        public static bool In(this Range from, int to) 
            => from.Start.Value <= to && from.End.Value >= to;
        public static bool In(this ushort from, Range to) 
            => to.Start.Value <= from && to.End.Value >= from;
    }
}