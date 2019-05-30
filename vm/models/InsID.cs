namespace vm.models
{
    public enum InsID : short
    {
        warm,
        loadi,
        add,
        sub,
        div,
        mul,
        pow,
        push_a,
        push_d,
        swap,

        ref_t,
        jump_t,

        mov_d,

        halt
    }
}