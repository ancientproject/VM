#include "../header/terminalDev.h";
#include <iostream>
#include "../header/utils.hpp"

termDev::termDev()
{
    set_addr(0x1);
    set_name("terminal");
}

void termDev::write(int address, int data)
{
    std::cout << "term 0x" << i32hex(&address, sizeof address) << " 0x" << i32hex(&data, sizeof data) << endl;
    switch (address)
    {
        case 0x5:
            cout << data;
            break;
    }
}

