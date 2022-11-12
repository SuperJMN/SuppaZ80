using System.Collections.Generic;
using Konamiman.Z80dotNet;

namespace SuppaZ80.ViewModels;

public class Status
{
    public Status(IEnumerable<byte> memory, IZ80Registers registers)
    {
        var regs = new[]
        {
            new Register("AF", registers.AF),
            new Register("BC", registers.BC),
            new Register("DE", registers.DE),
            new Register("HL", registers.HL),
            new Register("PC", registers.PC),
            new Register("SP", registers.SP),
            new Register("I", registers.SP)
        };

        RawMemory = memory;
        RawRegisters = registers;
    }

    public IZ80Registers RawRegisters { get; }

    public IEnumerable<byte> RawMemory { get; }
}