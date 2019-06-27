namespace ancient.compiler.tokens
{
    using System.Linq;

    public class PushJEvolve : ClassicEvolve
    {
        public PushJEvolve(string value, byte cellDev, byte ActionDev)
        {
            Result = value.Select(x => $".push_a &(0x{cellDev:X1}) &(0x{ActionDev:X1}) <| $(0x{(ushort)x:X})").ToArray();
        }
    }
}