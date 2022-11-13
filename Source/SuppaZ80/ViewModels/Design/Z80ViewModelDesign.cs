using System.Collections.Generic;
using System.Reactive;
using Konamiman.Z80dotNet;
using ReactiveUI;

namespace SuppaZ80.ViewModels.Design;

internal class Z80ViewModelDesign : IZ80ViewModel
{
    public ReactiveCommand<Unit, Unit> Stop => ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, Status> Run => ReactiveCommand.Create(() => new Status(new List<byte>(), new Z80Registers()));
}