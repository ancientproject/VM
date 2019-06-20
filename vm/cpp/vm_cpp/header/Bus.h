#pragma once
#include <map>
using namespace std;
#ifndef __BUS__
#define __BUS__
#include "abstractDevice.h"
#include <list>
#include <vector>

class State;
class CPU;
class Bus
{
public:
    State* state;
    CPU* cpu;
    int* boundaries = new int[16];
    map<short, abstractDevice> devices = {};

    void write(short address, int data) const;
    int read(short address);
    void add(const abstractDevice* dev) const;
    abstractDevice* find(int address) const;

    void setup(CPU* cpu, State* state);
};

#endif