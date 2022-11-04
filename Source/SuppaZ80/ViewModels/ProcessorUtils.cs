using System.Collections.Immutable;
using Konamiman.Z80dotNet;

namespace SuppaZ80.ViewModels;

public static class ProcessorUtils
{
    public static ProcessorStatus GetStatus(this IZ80Processor z80)
    {
        return new ProcessorStatus(z80.Memory.GetContents(0, z80.Memory.Size).ToImmutableArray(), z80.Registers);
    }
}