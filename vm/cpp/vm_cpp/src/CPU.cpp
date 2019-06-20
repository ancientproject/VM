#include <iostream>
#include "../header/CPU.h"
#include "../header/utils.hpp"
#include "../header/State.h"


CPU::CPU(Bus* bus)
{
    this->_bus = bus;
}

void CPU::step() const
{
    this->_bus->state->accept(this->_bus->state->fetch());
    this->_bus->state->eval();
}

void CPU::halt(uint8_t code)
{
    cout << red 
    << "cpu_.halt 0x" 
    << u32hex(reinterpret_cast<uint32_t*>(code), sizeof(code)) 
    << white << endl;
}
