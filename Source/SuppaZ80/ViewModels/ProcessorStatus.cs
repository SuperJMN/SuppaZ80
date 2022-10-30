using System.Collections.Generic;
using System.Linq;
using Konamiman.Z80dotNet;

namespace SuppaZ80.ViewModels;

public class ProcessorStatus
{
    public ProcessorStatus(IEnumerable<byte> memory, IZ80Registers registers)
    {
        Registers = new List<RegisterViewModel>
        {
            new ShortRegisterViewModel("AF", registers.AF),
            new ShortRegisterViewModel("BC", registers.BC),
            new ShortRegisterViewModel("DE", registers.DE),
            new ShortRegisterViewModel("HL", registers.HL),
            new ShortRegisterViewModel("PC", (short) registers.PC),
            new ShortRegisterViewModel("SP", registers.SP),
            new ShortRegisterViewModel("I", registers.I)
        };

        Memory = memory.Select((b, i) => new MemoryViewModel(b, i)).ToList();
    }

    public IEnumerable<MemoryViewModel> Memory { get; }
    public List<RegisterViewModel> Registers { get; }
}