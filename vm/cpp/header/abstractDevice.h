#pragma once
#include <sstream>
using namespace std;
/**
 * \brief declare get/set field
 * \param type type value
 * \param var variable name
 */
#define GSF(type, var)  private: type var; \
    public: type get_##var() const { return var; } \
            void set_##var(type val) { (var) = val; }

class abstractDevice  
{
    GSF(short, addr);
    GSF(string, name);
public:
    virtual ~abstractDevice() = default;
    virtual void write(int address, int data);
    virtual int read(int address);
    virtual void init();
    virtual void shutdown();
};

