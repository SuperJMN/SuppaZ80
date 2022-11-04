using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using ReactiveUI;
using Sixty502DotNet;

namespace SuppaZ80.ViewModels;

public class Z80StepViewModel : ViewModelBase, IZ80StepViewModel
{
    private readonly Z80Processor z80 = new();

    public Z80StepViewModel(IObservable<AssemblyData> program)
    {
        program
            .Do(assemblyData =>
            {
                z80.Reset();
                z80.Memory.SetContents(0, assemblyData.ProgramBinary);
            })
            .Subscribe();

        Step = ReactiveCommand.CreateFromObservable(() => Observable.Return(z80.ExecuteNextInstruction()).Select(_ => ProcessorUtils.GetStatus(z80)));

        Reset = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => z80.Reset()).Select(_ => ProcessorUtils.GetStatus(z80)));

        CurrentLine = Reset.Merge(Step).WithLatestFrom(program, (status, data) =>
        {
            var line = Maybe
                .From(status.Registers["PC"])
                .Bind(r => data.DebugInfo.TryFirst(x => x.ProgramCounter == r.Value))
                .Select(x => x.Line);

            return line;
        }).Select(x => x.Match(i => i.ToString(), () => ""));
    }

    public ReactiveCommand<Unit, ProcessorStatus> Reset { get; }

    public IObservable<string> CurrentLine { get; }

    public ReactiveCommand<Unit, ProcessorStatus> Step { get; }
}