#include "../header/abstractDevice.h"
#include <exception>
#include <iostream>

void abstractDevice::write(int address, int data)
{
    std::cout << "err -> write to null" << endl;
}
int abstractDevice::read(int address)
{
    return 0;
}
void abstractDevice::init() { }

void abstractDevice::shutdown() { }
