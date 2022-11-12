using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public interface IDebugger
{
    ReactiveCommand<Unit, Unit> Stop { get; }
    ReactiveCommand<Unit, Status> Step { get; }
    IObservable<Maybe<int>> CurrentLine { get; }
    ReactiveCommand<Unit, Status> Play { get; }
    IObservable<Status> StatusChanged { get; }
    IObservable<bool> IsDebugging { get; }
    void SetMemory(int location, byte value);
    void SetRegister(string name, short value);
    void SetRegister(string name, ushort value);
}
