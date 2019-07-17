namespace ancient.runtime
{
    public enum IID : short
    {
        [OpCode(0x00)] nop,
        [OpCode(0x0A)] warm,
        [OpCode(0x0D)] halt,

        [OpCode(0x01)] ldi,
        [OpCode(0x01)] ldx,

        [OpCode(0x0F)] mva, 
        [OpCode(0x0F)] mvd,
        [OpCode(0x0F)] mvx,
        [OpCode(0xA4)] rfd,

        [OpCode(0x03)] swap,

        [OpCode(0x08)] ref_t , [OpCode(0x08)] jump_t, 
        [OpCode(0x08)] jump_e, [OpCode(0x08)] jump_g,
        [OpCode(0x08)] jump_u, [OpCode(0x08)] jump_y,
        [OpCode(0x09)] jump_p,


        [OpCode(0x33)] page,

        [OpCode(0x34)] lpstr,
        [OpCode(0x35)] unlock,
        [OpCode(0x36)] call_i,
        [OpCode(0x37)] prune,
        [OpCode(0x38)] locals,
        

        [UnfCode] mvj, [UnfCode] raw,

        [OpCode(0xA0)] orb,
        [OpCode(0xA1)] pull,
        [OpCode(0xAA)] val,

        [OpCode(0xB1)] inc,
        [OpCode(0xB2)] dec,
        [OpCode(0xB3)] dup,
        [OpCode(0xB4)] ckft,

        [OpCode(0xC1)] brk_s,
        [OpCode(0xC1)] brk_n,
        [OpCode(0xC1)] brk_a,

        //[OpCode(0xA4)] inv,
        //[OpCode(0xA5)] sig, 
        //[OpCode(0xA6)] ret,

        

        // 1x, abs, acos, atan, acosh, atanh, asin, asinh, cbrt, cell, cos, cosh, flr, exp, log, log10, tan, tanh, trc, bitd, biti
        // 2x, atan2, min, max


        [OpCode(0xCA)] add, 
        [OpCode(0xCB)] sub,
        [OpCode(0xCC)] div, 
        [OpCode(0xCD)] mul,

        [OpCode(0xCE)] pow, 
        [OpCode(0xCF)] sqrt,

        [OpCode(0xD0)] abs, 
        [OpCode(0xD1)] acos, 
        [OpCode(0xD2)] atan,
        [OpCode(0xD3)] acosh, 
        [OpCode(0xD4)] atanh, 
        [OpCode(0xD5)] asin, 
        [OpCode(0xD6)] asinh,
        [OpCode(0xD7)] cbrt,
        [OpCode(0xD8)] cell,
        [OpCode(0xD9)] cos,
        [OpCode(0xDA)] cosh,
        [OpCode(0xDB)] flr,
        [OpCode(0xDC)] exp,
        [OpCode(0xDD)] log,
        [OpCode(0xDE)] log10,
        [OpCode(0xDF)] tan,
        [OpCode(0xE0)] tanh,
        [OpCode(0xE1)] trc,
        [OpCode(0xE2)] bitd,
        [OpCode(0xE3)] biti, 

        [OpCode(0xE4)] atan2,
        [OpCode(0xE5)] min,
        [OpCode(0xE6)] max,
        [OpCode(0xE7)] sin,
        [OpCode(0xE8)] sinh,
    }
}