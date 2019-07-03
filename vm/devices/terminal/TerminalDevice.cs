using System;
using System.Runtime.InteropServices;
using System.Text;
using ancient.runtime;
using static System.Console;


[Guid("2F380069-1951-41DF-9500-3FE4610FA924")]
public class TerminalDevice : IDevice
{
    private StringBuilder relMemory = new StringBuilder();


    public string name { get; } = "term";
    public short startAddress { get; } = 0x1;
    public void write(long address, long data)
    {
        switch (address)
        {
            case 0x5:
                char c1 = (char)data;
                if(c1 == 0xA)
                    Write(Environment.NewLine);
                else 
                    Write(c1);
                break;
            case 0x6:
                char c2 = (char)data;
                if (c2 == 0xA)
                    relMemory.Append(Environment.NewLine);
                else 
                    relMemory.Append(c2);
                break;
            case 0x7:
                Out.Write(relMemory);
                break;
            case 0x8:
                relMemory.Clear();
                break;
            case 0x9:
                var u1 = ((short)data & 0xF0) >> 4;
                var u2 = (short)data & 0xF;

                switch (u1)
                {
                    case 0x1:
                        ForegroundColor = (ConsoleColor)u2;
                        break;
                    case 0x2:
                        BackgroundColor = (ConsoleColor)u2;
                        break;
                    case 0x3:
                        ForegroundColor = ConsoleColor.White;
                        break;
                    case 0x4:
                        BackgroundColor = ConsoleColor.Black;
                        break;
                }
                break;
        }
    }

    public long read(long address)
    {
        throw new NotImplementedException();
    }

    public long this[long address]
    {
        get => read(address);
        set => write(address, value);
    }

    public void warmUp() {}

    public void shutdown() => relMemory = null;

    public int CompareTo(object obj)
    {
        if (!(obj is IDevice dev)) return 0;
        if (startAddress > dev.startAddress)
            return 1;
        return -1;
    }
    public override int GetHashCode() => 
        startAddress.GetHashCode() ^ 42 * 
        name.GetHashCode() ^ 42;
}