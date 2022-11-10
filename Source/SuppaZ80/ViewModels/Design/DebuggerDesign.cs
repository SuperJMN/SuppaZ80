using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using ReactiveUI;

namespace SuppaZ80.ViewModels.Design;

internal class DebuggerDesign : IDebugger
{
    public ReactiveCommand<Unit, Unit> Stop => ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, ProcessorStatus> Step => ReactiveCommand.Create(() => new ProcessorStatus(new List<byte>(), new Z80Registers()));
    public IObservable<Maybe<int>> CurrentLine => Observable.Return(Maybe<int>.From(3));
    public ReactiveCommand<Unit, ProcessorStatus> Play { get; }
    public IObservable<ProcessorStatus> Status { get; }
    public IObservable<bool> IsDebugging => Observable.Return(false);

    public void SetMemory(int location, byte value)
    {
    }
}