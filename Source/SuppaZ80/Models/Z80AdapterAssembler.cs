using CSharpFunctionalExtensions;
using Sixty502DotNet;

namespace SuppaZ80.Models;

public class Z80AdapterAssembler : IAssembler
{
    public Result<byte[]> Assemble(string code)
    {
        var assembler = new Z80Assembler();
        return assembler.Assemble(code);
    }
}