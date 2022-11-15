using System;
using System.Reactive;
using System.Reactive.Linq;
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

    public Debugger(IObservable<Result<AssemblyData>> assembleResultChanged)
    {
        prop = assembleResultChanged.ToProperty(this, x => x.AssembleResult);
        Play = ReactiveCommand.CreateFromObservable(() => this.WhenAnyValue(x => x.AssembleResult).WhereSuccess().SelectMany(result => StartDebugSession(z80, result)));
        Step = ReactiveCommand.Create(() => Status.Empty);
        Stop = ReactiveCommand.Create(() => { });
        IsDebugging = Play.To(true).Merge(Stop.To(false)).DistinctUntilChanged();
        StatusChanged = Observable.Return(Status.Empty);
    }

    public Result<AssemblyData> AssembleResult => prop.Value;

    private IObservable<Status> StartDebugSession(Z80Processor z80Processor, AssemblyData assemblyData)
    {
        z80Processor.Memory.SetContents(0, assemblyData.ProgramBinary);
        z80Processor.Registers = new Z80Registers();

        return Observable.Return(z80Processor.GetStatus());
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