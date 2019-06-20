#include "../header/Bus.h"
#include "../header/CPU.h"
#include "../header/State.h"
#include <exception>

void Bus::write(const short address, const int data) const
{
    auto dev = this->find(address);
    dev->write(address - dev->get_addr(), data);
}

int Bus::read(short address)
{
    throw new std::exception(&"access denied: "[address]);
}

void Bus::add(const abstractDevice* dev) const
{
    // FUCK
    auto ths = *this; // FUCKING
    // C++ COLLECTION
    auto dict = this->devices; // FUCKIGN SHIT
    dict.insert(make_pair(dev->get_addr(), *dev)); // FUCK IT
    ths.devices = dict; // HATE 
}

abstractDevice* Bus::find(int address) const
{
    auto dict = this->devices;
    return &dict.at(static_cast<short>(address));
}

void Bus::setup(CPU* cpu, State* state)
{
    this->cpu = cpu;
    this->state = state;
}
