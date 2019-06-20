#include <iostream>
#include "../header/State.h"
#include "../header/termcolor.hpp"
#include "../header/CPU.h"
#include "../header/terminalDev.h"

int main()
{
    SetConsoleTitle(L"cpu host");
    auto s = new Bus();
    const auto term = new termDev();
    s->setup(new CPU(s), new State(s));
    s->add(term);
    const auto prog = new uint32_t[8];
    prog[0x0] = 0xABCDEFE0;
    prog[0x1] = 0xF150078C;
    prog[0x2] = 0xF150075C;
    prog[0x3] = 0xF150079C;
    prog[0x4] = 0xF15000AC;
    s->state->load(prog);
    auto len = sizeof(prog);
    for(auto i = 0; i != sizeof(prog); i++)
        s->cpu->step();
    
    system("pause");
}
