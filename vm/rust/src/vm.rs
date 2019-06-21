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
    pub last_addr: u32,

    pub memory: [u32; 512],
    pub regs: [u32; 32],
    pub bus: Bus
}

impl State
{
    pub fn new() -> State
    {
        State {
            r1: 0x0, r2: 0x0,
            r3: 0x0, u1: 0x0,
            u2: 0x0, x1: 0x0,
            x2: 0x0, iid: 0x0,
            pc: 0x0, halt: 0x0,
            curr_addr: 0x0, last_addr: 0x0,
            memory: [],
            regs: [0x0; 32],
            bus: Bus
        }
    }
    pub fn accept(&mut self, mem: u32)
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
    pub fn fetch(&mut self) -> u32
    {
        self.last_addr = self.curr_addr;
        if self.memory.len() != self.pc || self.halt != 0
        {
            self.pc += 1;
            self.curr_addr = self.memory[self.pc];
            return self.curr_addr;
        }
        return self.memory[self.pc];
    }

    pub fn eval(&mut self)
    {
        let p = (self.iid, self.x1, self.x2);
        match p {
            (0xA, _, _) => println!("warm up"),
            (0x1, _, 0x0) => {
                match self.u2 {
                    0x0 => self.regs[self.r1] = self.u1,
                    _   => self.regs[self.r1] = (self.u1 << 4) | self.u2
                }
            },
            (0x1, _, 0xA) => self.regs[(self.r1 << 4) | self.r2] = (self.u1 << 4) | self.u2,
            (0x2, _, _)  => self.regs[self.r1] = self.memory[self.r2] + self.memory[self.r3],
            (0x3, _, _)  => {
                self.regs[self.r1] ^= self.regs[self.r2];
                self.regs[self.r2] = self.regs[self.r1] ^ self.regs[self.r2];
                self.regs[self.r1] ^= self.regs[self.r2];
            },
            (0xF, _, _)  => self.bus.find(self.r1 & 0xFF).write(self.r2 & 0xFF, self.regs[self.r3] & 0xFF)
        }
    }
}

use std::fmt;

impl fmt::LowerHex for State
{
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "({}, {})", self.r1, self.r2)
    }
}
