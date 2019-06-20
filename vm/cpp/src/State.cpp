#include <iostream>
#include "../header/State.h"
#include "../header/boolinq.h"
#include "../header/utils.hpp"
#include "../header/termcolor.hpp"
#include "../header/CPU.h"

using namespace boolinq;
void State::load(uint32_t memory[])
{
    this->mem = memory;
}

uint32_t State::fetch()
{
    this->lastAddr = this->curAddr;
    if(sizeof this->mem != this->pc || this->halt != 0)
        return this->curAddr = this->mem[this->pc++];
    return this->mem[this->pc];
}
void State::eval() const
{
    const auto regs = {r1, r2, r3, u1, u2, x1, x2};
    if (iid == 0xA)
        std::cout << yellow << "warm-up" << white << endl;
    switch (iid)
    {
        case 0xA: break;
        case 0xF:
            if(from(regs).all([](const uint8_t x){ return x == 0xF; }))
            {
                CPU::halt(0x0);
                break;
            }
            if(x2 == 0xC)
            {
                std::cout << "mva " << u8hex(&r1, sizeof r1) << u8hex(&r2, sizeof r2) << endl;
                std::cout << "mva " << u8hex(&r3, sizeof r3) << u8hex(&u1, sizeof u1) << u8hex(&u2, sizeof u2) << endl;
                this->_bus->find(r1 & 0xFF)->
                    write(r2 & 0xFF, (r3 << 12 | u1 << 8 | u2 << 4 | x1) & 0xFFFFFFF);
            }
            break;
        case 0x1:
            std::cout << "loadi " << u8hex(&u1, sizeof u1) << u8hex(&u2, sizeof u2) << endl;
            if (u2 != 0)
                reg[r1] = (u1 << 4) | u2;
            else
                reg[r1] = u1;
            break;
        case 0x2:
            std::cout << "swp " << u8hex(&r1, sizeof r1) << u8hex(&r2, sizeof r2) << endl;
            reg[r1] ^= reg[r2];
            reg[r2] = reg[r1] ^ reg[r2];
            reg[r1] ^= reg[r2];
            break;
        case 0x3:
            reg[r1] = reg[r2] + reg[r3];
            break;
        case 0x4:
            reg[r1] = reg[r2] - reg[r3];
            break;
        case 0x5:
            reg[r1] = reg[r2] * reg[r3];
            break;
        case 0x6:
            if (reg[r3] == 0x0)
            {
                CPU::halt(0xC);
                break;
            }
            reg[r1] = reg[r2] / reg[r3];
            break;
        default: 
            cout << red << "unk op-code: " << u32hex(&iid, sizeof iid) << white << endl;
        break;
    }
}

void State::accept(uint32_t memory)
{
    iid = (memory & 0xF0000000) >> 28;
    r1  = (memory & 0xF000000 ) >> 24;
    r2  = (memory & 0xF00000  ) >> 20;
    r3  = (memory & 0xF0000   ) >> 16;
    u1  = (memory & 0xF000    ) >> 12;
    u2  = (memory & 0xF00     ) >> 8;
    x1  = (memory & 0xF0      ) >> 4;
    x2  =  memory & 0xF;
}

State::State(Bus* bus): 
    r1(0), r2(0), r3(0), u1(0), u2(0), x1(0), x2(0), halt(0), pc(0), iid(0), tc(0), ec(0), km(0), wf(0)
{
    this->_bus = bus;
}
