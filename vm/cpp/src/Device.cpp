#include "../header/Device.h"
#include <exception>
#include <iostream>

void Device::write(int address, int data)
{
    std::cout << "err -> write to null" << endl;
}
int Device::read(int address)
{
    return 0;
}
void Device::init() { }

void Device::shutdown() { }
