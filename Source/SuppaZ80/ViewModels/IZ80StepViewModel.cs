using System;
using System.Reactive;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public interface IZ80StepViewModel
{
    ReactiveCommand<Unit, ProcessorStatus> Reset { get; }
    ReactiveCommand<Unit, ProcessorStatus> Step { get; }
    IObservable<string> CurrentLine { get; }
}