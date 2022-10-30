using System.Reactive;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public interface IZ80StepViewModel
{
    ReactiveCommand<Unit, Unit> Reset { get; }
    ReactiveCommand<Unit, ProcessorStatus> Step { get; }
}