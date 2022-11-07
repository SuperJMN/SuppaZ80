using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public interface IDebugger
{
    ReactiveCommand<Unit, Unit> Stop { get; }
    ReactiveCommand<Unit, ProcessorStatus> Step { get; }
    IObservable<Maybe<int>> CurrentLine { get; }
    ReactiveCommand<Unit, ProcessorStatus> Play { get; }
    IObservable<ProcessorStatus> Status { get; }
    IObservable<bool> IsDebugging { get; }
    void SetMemory(int location, byte value);
}