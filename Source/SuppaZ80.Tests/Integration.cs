using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Sixty502DotNet;
using Z80 = SuppaZ80.Models.Z80;

namespace SuppaZ80.Tests;

public class Integration
{
    [Fact]
    public async Task Run_until_halted()
    {
        var assembler = new Z80Assembler();
        await Z80.RunUntilHalted(assembler.Assemble("HALT").Value, new NewThreadScheduler());
    }

    [Fact]
    public async Task Run_until_halted_should_stop()
    {
        var assembler = new Z80Assembler();
        await Z80.RunUntilHalted(assembler.Assemble("\tLD hl,0").Value, new NewThreadScheduler(), Observable.Timer(TimeSpan.FromSeconds(5)));
    }
}