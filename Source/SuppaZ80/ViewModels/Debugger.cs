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

    public Debugger(IObservable<Result<AssemblyData>> assembleResultChanged)
    {
        assembleResultChanged
            .WhereSuccess()
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
        StatusChanged = Step.Merge(Play);
        Play.Select(_ => false).Subscribe(canStartDebuggingSession);
        Stop.Select(_ => true).Subscribe(canStartDebuggingSession);

        Step.Select(_ => z80.IsHalted).Merge(Stop.Select(_ => false)).Subscribe(isHaltedSubject);

        var currentLine = Play.Merge(Step).WithLatestFrom(assembleResultChanged.WhereSuccess(), (status, data) =>
        {
            if (z80.IsHalted)
            {
                return Maybe<int>.None;
            }

            var line = Maybe
                .From(status.RawRegisters.PC)
                .Bind(pc => data.DebugInfo.TryFirst(x => x.ProgramCounter == pc))
                .Select(x => x.Line);

            return line;
        });

        var previous = currentLine.SkipLast(1).StartWith(Maybe<int>.None);

        CurrentLine = currentLine.WithLatestFrom(previous, (c, p) => c.HasNoValue ? p : c).Merge(Stop.Select(_ => Maybe<int>.None));
    }

    public IObservable<bool> IsDebugging { get; }

    public void SetMemory(int location, byte value)
    {
        z80.Memory[location] = value;
    }

    public void SetRegister(string name, short value)
    {
        switch (name)
        {
            case "AF":
                z80.Registers.AF = value;
                break;
            case "BC":
                z80.Registers.BC = value;
                break;
            case "DE":
                z80.Registers.DE = value;
                break;
            case "HL":
                z80.Registers.HL = value;
                break;
        }
    }

    public void SetRegister(string name, ushort value)
    {
        switch (name)
        {
            case "PC":
                z80.Registers.PC = value;
                break;
        }
    }

    public IObservable<Status> StatusChanged { get; }

    public ReactiveCommand<Unit, Status> Play { get; }

    public ReactiveCommand<Unit, Unit> Stop { get; }

    public IObservable<Maybe<int>> CurrentLine { get; }

    public ReactiveCommand<Unit, Status> Step { get; }
}