namespace ancient.runtime
{
    public class jump_t : jump
    {
        public jump_t(byte cell) : base(cell, 0x0, 0x0, 0x0, InsID.jump_t) {}
    }
    public class jump_e : jump
    {
        public jump_e(byte cell, byte f_cell, byte t_cell) : base(cell, f_cell, t_cell, 0x1, InsID.jump_e) {}
    }
    public class jump_g : jump
    {
        public jump_g(byte cell, byte f_cell, byte t_cell) : base(cell, f_cell, t_cell, 0x2, InsID.jump_g) {}
    }
    public class jump_u : jump
    {
        public jump_u(byte cell, byte f_cell, byte t_cell) : base(cell, f_cell, t_cell, 0x3, InsID.jump_u) {}
    }
    public class jump_y : jump
    {
        public jump_y(byte cell, byte f_cell, byte t_cell) : base(cell, f_cell, t_cell, 0x4, InsID.jump_y) {}
    }

    public abstract class jump : Instruction
    {
        internal readonly byte _cell;
        internal readonly byte _r2;
        internal readonly byte _r3;
        internal readonly byte _x1;

        protected jump(byte cell, byte r2, byte r3, byte x1, InsID id) : base(id)
        {
            _cell = cell;
            _r2 = r2;
            _r3 = r3;
            _x1 = x1;
        }
        protected override void OnCompile() => SetRegisters(_cell, r2: _r2, r3: _r3, u2: 0xF,  x1: _x1);
    }
}