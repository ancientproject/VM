#include "../header/Bus.h"
#include "../header/CPU.h"
#include "../header/State.h"
#include <exception>

map<short, Device> Bus::devices;

void Bus::write(const short address, const int data) const
{
    auto dev = this->find(address);
    dev->write(address - dev->get_addr(), data);
}

int Bus::read(short address)
{
    throw new std::exception(&"access denied: "[address]);
}

void Bus::add(const Device* dev) const
{
    devices.insert(make_pair(dev->get_addr(), *dev));
}

Device* Bus::find(int address) const
{
    return &devices.at(static_cast<short>(address));
}

void Bus::setup(CPU* cpu, State* state)
{
    this->cpu = cpu;
    this->state = state;
}
