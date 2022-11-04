using CSharpFunctionalExtensions;
using Sixty502DotNet;

namespace SuppaZ80.Models;

public interface IAssembler
{
    Result<AssemblyData> Assemble(string code);
}