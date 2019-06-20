#pragma once
#include "Device.h"


class termDev : public Device
{
public:
    termDev();
    void write(int address, int data) override;
};
