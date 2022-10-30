using System;
using System.Reactive;
using System.Reactive.Linq;
using Konamiman.Z80dotNet;
using ReactiveUI;

namespace SuppaZ80.ViewModels;

public class Z80StepViewModel : ViewModelBase, IZ80StepViewModel
{
    private readonly Z80Processor z80 = new();

    public Z80StepViewModel(IObservable<byte[]> program)
    {
        program
            .Do(bytes =>
            {
                z80.Reset();
                z80.Memory.SetContents(0, bytes);
            })
            .Subscribe();

        Step = ReactiveCommand.CreateFromObservable(() => Observable.Return(z80.ExecuteNextInstruction()).Select(_ => ProcessorUtils.GetStatus(z80)));
        Reset = ReactiveCommand.Create(() => z80.Reset());
    }

    public ReactiveCommand<Unit, Unit> Reset { get; }

    public ReactiveCommand<Unit, ProcessorStatus> Step { get; }
}