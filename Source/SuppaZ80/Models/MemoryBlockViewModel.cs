using System.Collections.Generic;
using SuppaZ80.ViewModels;

namespace SuppaZ80.Models;

public class MemoryBlockViewModel
{
    public IEnumerable<IndexedMemory> Memory { get; }
    public int BaseAddress { get; }

    public MemoryBlockViewModel(IEnumerable<IndexedMemory> memory, int baseAddress)
    {
        Memory = memory;
        BaseAddress = baseAddress;
    }
}