#include <iostream>
#include "../header/State.h"
#include "../header/termcolor.hpp"
#include "../header/CPU.h"
#include "../header/terminalDev.h"

int main()
{
    SetConsoleTitle(L"cpu host");
    auto s = new Bus();
    s->setup(new CPU(s), new State(s));
    const auto term = new termDev();
    s->add(term);
    const auto prog = {
        0xABCDEFE0, 
        0xF150078C, 
        0xF150075C,
        0xF150079C,
        0xF15000AC
    };
    s->state->load(prog);
    for(auto i = 0; i != prog.size(); i++)
        s->cpu->step();
    system("pause");
}
