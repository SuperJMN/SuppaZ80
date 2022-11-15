using System.Collections.Generic;
using System.Linq;
using Konamiman.Z80dotNet;

namespace SuppaZ80.ViewModels;

public class Status
{
    public Status(IEnumerable<byte> memory, IZ80Registers registers)
    {
        RawMemory = memory;
        RawRegisters = registers;
    }

    public IZ80Registers RawRegisters { get; }

    public IEnumerable<byte> RawMemory { get; }

    public static Status Empty = new(Enumerable.Empty<byte>(), new Z80Registers());
}