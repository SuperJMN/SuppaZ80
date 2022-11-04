using System.Collections.Generic;
using System.Linq;
using Konamiman.Z80dotNet;

namespace SuppaZ80.ViewModels;

public class ProcessorStatus
{
    public ProcessorStatus(IEnumerable<byte> memory, IZ80Registers registers)
    {
        var regs = new[]
        {
            new Register("AF", registers.AF),
            new Register("BC", registers.BC),
            new Register("DE", registers.DE),
            new Register("HL", registers.HL),
            new Register("PC", registers.PC),
            new Register("SP", registers.SP),
            new Register("I", registers.I)
        };

        Registers = new Registers(regs);

        Memory = memory.Select((b, i) => new MemoryViewModel(b, i)).ToList();
    }

    public IEnumerable<MemoryViewModel> Memory { get; }
    public Registers Registers { get; }
}