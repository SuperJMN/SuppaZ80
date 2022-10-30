using System.Collections.Generic;
using System.Reactive;
using Konamiman.Z80dotNet;
using ReactiveUI;

namespace SuppaZ80.ViewModels.Design;

class Z80StepViewModelDesign : IZ80StepViewModel
{
    public ReactiveCommand<Unit, Unit> Reset => ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, ProcessorStatus> Step => ReactiveCommand.Create(() => new ProcessorStatus(new List<byte>(), new Z80Registers()));
}