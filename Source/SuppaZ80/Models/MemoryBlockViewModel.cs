using System.Collections.Generic;
using SuppaZ80.ViewModels;

namespace SuppaZ80.Models;

public class MemoryBlockViewModel
{
    public IEnumerable<MemoryViewModel> Memory { get; }
    public int BaseAddress { get; }

    public MemoryBlockViewModel(IEnumerable<MemoryViewModel> memory, int baseAddress)
    {
        Memory = memory;
        BaseAddress = baseAddress;
    }
}