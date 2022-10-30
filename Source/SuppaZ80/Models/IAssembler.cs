using CSharpFunctionalExtensions;

namespace SuppaZ80.Models;

public interface IAssembler
{
    Result<byte[]> Assemble(string code);
}