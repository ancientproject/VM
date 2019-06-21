#[derive(Debug)]
pub struct State {
    pub r1: u32,
    pub r2: u32,
    pub r3: u32,

    pub u1: u32,
    pub u2: u32,
    pub x1: u32,
    pub x2: u32,

    pub iid: u32,
    pub pc: u32,
    pub halt: u8,
    pub curr_addr: u32,
    pub last_addr: u32
}

impl State {
    pub fn new() -> State {
        State {
            r1: 0x0, r2: 0x0,
            r3: 0x0, u1: 0x0,
            u2: 0x0, x1: 0x0,
            x2: 0x0, iid: 0x0,
            pc: 0x0, halt: 0x0,
            curr_addr: 0x0, last_addr: 0x0
        }
    }
    pub fn fetch(&mut self, mem: u32)
    {
        self.iid = (mem & 0xF0000000) >> 28;
        self.r1  = (mem & 0xF000000 ) >> 24;
        self.r2  = (mem & 0xF00000  ) >> 20;
        self.r3  = (mem & 0xF0000   ) >> 16;
        self.u1  = (mem & 0xF000    ) >> 12;
        self.u2  = (mem & 0xF00     ) >> 8;
        self.x1  = (mem & 0xF0      ) >> 4;
        self.x2  = (mem & 0xF       ) >> 0;
    }
}

use std::fmt;

impl fmt::LowerHex for State {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "({}, {})", self.r1, self.r2)
    }
}
