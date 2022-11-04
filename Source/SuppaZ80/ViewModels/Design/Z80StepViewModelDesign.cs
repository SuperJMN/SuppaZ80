using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Konamiman.Z80dotNet;
using ReactiveUI;

namespace SuppaZ80.ViewModels.Design;

class Z80StepViewModelDesign : IZ80StepViewModel
{
    public ReactiveCommand<Unit, ProcessorStatus> Reset => ReactiveCommand.Create(() => new ProcessorStatus(new List<byte>(), new Z80Registers()));
    public ReactiveCommand<Unit, ProcessorStatus> Step => ReactiveCommand.Create(() => new ProcessorStatus(new List<byte>(), new Z80Registers()));
    public IObservable<string> CurrentLine => Observable.Return("3");
}