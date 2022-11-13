using System.Reactive;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public interface IZ80ViewModel
{
    ReactiveCommand<Unit, Unit> Stop { get; }
    ReactiveCommand<Unit, Status> Run { get; }
}