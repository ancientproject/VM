#pragma once
#include "abstractDevice.h"


class termDev : public abstractDevice
{
public:
    termDev();
    void write(int address, int data) override;
};
