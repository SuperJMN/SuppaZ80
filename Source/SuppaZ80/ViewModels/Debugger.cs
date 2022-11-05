using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using ReactiveUI;
using Sixty502DotNet;

namespace SuppaZ80.ViewModels;

public class Debugger : ViewModelBase, IDebugger
{
    private readonly Z80Processor z80 = new();

    public Debugger(IObservable<AssemblyData> program)
    {
        program
            .Do(assemblyData =>
            {
                z80.Reset();
                z80.Memory.SetContents(0, assemblyData.ProgramBinary);
            })
            .Subscribe();

        var canStartDebuggingSession = new BehaviorSubject<bool>(true);
        var isHaltedSubject = new BehaviorSubject<bool>(false);

        IsDebugging = canStartDebuggingSession.Select(b => !b);

        Step = ReactiveCommand.CreateFromObservable(() => Observable.Return(z80.ExecuteNextInstruction()).Select(_ => z80.GetStatus()), isHaltedSubject.Select(x => !x));


        Stop = ReactiveCommand.Create(() => { }, canStartDebuggingSession.Select(b => !b));
        Play = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => z80.Reset()).Select(_ => z80.GetStatus()), canStartDebuggingSession);
        Status = Step.Merge(Play);
        Play.Select(_ => false).Subscribe(canStartDebuggingSession);
        Stop.Select(_ => true).Subscribe(canStartDebuggingSession);

        Step.Select(_ => z80.IsHalted).Merge(Stop.Select(_ => false)).Subscribe(isHaltedSubject);

        var currentLine = Play.Merge(Step).WithLatestFrom(program, (status, data) =>
        {
            if (z80.IsHalted)
            {
                return Maybe<int>.None;
            }

            var line = Maybe
                .From(status.Registers["PC"])
                .Bind(r => data.DebugInfo.TryFirst(x => x.ProgramCounter == r.Value))
                .Select(x => x.Line);

            return line;
        });

        var previous = currentLine.SkipLast(1).StartWith(Maybe<int>.None);

        CurrentLine = currentLine.WithLatestFrom(previous, (c, p) => c.HasNoValue ? p : c).Merge(Stop.Select(_ => Maybe<int>.None));
    }

    public IObservable<bool> IsDebugging { get; }

    public IObservable<ProcessorStatus> Status { get; }

    public ReactiveCommand<Unit, ProcessorStatus> Play { get; }

    public ReactiveCommand<Unit, Unit> Stop { get; }

    public IObservable<Maybe<int>> CurrentLine { get; }

    public ReactiveCommand<Unit, ProcessorStatus> Step { get; }
}