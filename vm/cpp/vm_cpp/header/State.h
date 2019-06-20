#pragma once
#ifndef __STATE__
#define __STATE__
#include <cstdint>
#include "Bus.h"

class State
{
private:
    static const int _32 = 32;
    Bus* _bus;
public:
    uint8_t r1, r2, r3;
    uint8_t u1, u2;
    uint8_t x1, x2;
    uint8_t halt;
    uint32_t pc, iid;

    char16_t tc, ec, km, wf;

    uint32_t curAddr = 0xFFFF;
    uint32_t lastAddr = 0xFFFF;

    uint32_t* mem = new uint32_t[_32];
    uint32_t* reg = new uint32_t[_32];


    void load(uint32_t memory[]);
    uint32_t fetch();
    void eval() const;
    void accept(uint32_t memory);

    explicit State(Bus* bus);
};

#endif