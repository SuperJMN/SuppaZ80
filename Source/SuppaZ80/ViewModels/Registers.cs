using System.Collections.Generic;
using System.Linq;

namespace SuppaZ80.ViewModels;

public class Registers : Dictionary<string, Register>
{
    public Registers(IEnumerable<Register> regs) : base(regs.ToDictionary(x => x.Name))
    {
    }
}