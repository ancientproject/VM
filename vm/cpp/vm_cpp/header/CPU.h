#pragma once
#ifndef __CPU__
#define __CPU__
#include <cstdint>
#include "./termcolor.hpp"
#include "./Bus.h";
using namespace std;
using namespace termcolor;
class CPU
{
private:
    Bus* _bus;
public:
    explicit CPU(Bus* bus);

    void step() const;
    static void halt(uint8_t code = 0x0);
};

#endif