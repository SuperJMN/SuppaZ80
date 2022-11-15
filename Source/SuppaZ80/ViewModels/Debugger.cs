using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using ReactiveUI;
using Sixty502DotNet;
using SuppaZ80.Models;

namespace SuppaZ80.ViewModels;

public class Debugger : ViewModelBase, IDebugger
{
    private readonly Z80Processor z80 = new();
    private readonly ObservableAsPropertyHelper<Result<AssemblyData>> prop;
    private readonly BehaviorSubject<bool> isDebugging = new(false);

    public Debugger(IObservable<Result<AssemblyData>> assembleResultChanged)
    {
        var isHalted = new BehaviorSubject<bool>(false);
        prop = assembleResultChanged.ToProperty(this, x => x.AssembleResult);
        Play = ReactiveCommand.CreateFromObservable(() => this.WhenAnyValue(x => x.AssembleResult).WhereSuccess().SelectMany(result => StartDebugSession(z80, result)).Take(1), isDebugging.Not());
        Step = ReactiveCommand.Create(() => z80.GetStatus());
        Stop = ReactiveCommand.Create(() => { }, isDebugging);
        Play.To(true).Merge(Stop.To(false)).StartWith(false).Subscribe(isDebugging);
        Step = ReactiveCommand.CreateFromObservable(() => Observable.Return(z80.ExecuteNextInstruction()).Select(_ => z80.GetStatus()), isDebugging.CombineLatest(isHalted, (a, b) => a && !b));
        StatusChanged = Play.Merge(Step);
        Step.Select(_ => z80.IsHalted).Merge(Stop.To(false)).Subscribe(isHalted);

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

    public Result<AssemblyData> AssembleResult => prop.Value;

    private IObservable<Status> StartDebugSession(Z80Processor z80Processor, AssemblyData assemblyData)
    {
        z80Processor.Reset();
        z80Processor.Memory.SetContents(0, assemblyData.ProgramBinary);
        z80Processor.Registers = new Z80Registers();

        return Observable.Return(z80Processor.GetStatus());
    }

    public IObservable<bool> IsDebugging => isDebugging;

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