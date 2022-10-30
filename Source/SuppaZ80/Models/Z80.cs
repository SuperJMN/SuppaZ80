using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Konamiman.Z80dotNet;
using SuppaZ80.ViewModels;

namespace SuppaZ80.Models;

public class Z80
{
    public static IObservable<ProcessorStatus> RunUntilHalted(byte[] program, IScheduler? scheduler)
    {
        return RunUntilHalted<Unit>(program, scheduler);
    }

    public static IObservable<ProcessorStatus> RunUntilHalted<T>(byte[] program, IScheduler? scheduler, IObservable<T>? until = null)
    {
        var z80 = new Z80Processor();
        z80.Memory.SetContents(0, program);

        return Observable
            .Generate(0, _ => !z80.IsHalted, x => x, _ => z80.ExecuteNextInstruction(), scheduler ?? Scheduler.Default)
            .TakeUntil(until ?? Observable.Never<T>())
            .TakeLast(1)
            .Select(_ => ProcessorUtils.GetStatus(z80));
    }
}