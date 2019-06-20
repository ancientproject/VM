#pragma once
#include <sstream>
#include <iomanip>

using namespace std;
inline string i32hex(const int *v, const int s) 
{
    stringstream ss;
    ss << hex << setfill('0');
    for (auto i = 0; i < s; i++)
      ss << hex << setw(2) << static_cast<int>(v[i]);
    return ss.str();
}
inline string u32hex(const uint32_t *v, const uint32_t s) 
{
    stringstream ss;
    ss << hex << setfill('0');
    for (auto i = 0; i < s; i++)
      ss << hex << setw(2) << static_cast<int>(v[i]);
    return ss.str();
}
inline string u8hex(const uint8_t *v, const uint8_t s) 
{
    stringstream ss;
    ss << hex << setfill('0');
    for (auto i = 0; i < s; i++)
      ss << hex << setw(2) << static_cast<int>(v[i]);
    return ss.str();
}
inline string u16hex(const uint16_t *v, const uint16_t s) 
{
    stringstream ss;
    ss << hex << setfill('0');
    for (auto i = 0; i < s; i++)
      ss << hex << setw(2) << static_cast<int>(v[i]);
    return ss.str();
}