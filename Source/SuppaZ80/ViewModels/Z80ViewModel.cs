using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using SuppaZ80.Models;

namespace SuppaZ80.ViewModels;

public class Z80ViewModel : ViewModelBase, IZ80ViewModel
{
    public Z80ViewModel(IAssembler assembler, IObservable<string> source)
    {
        var isRunning = new Subject<bool>();
        Stop = ReactiveCommand.Create(() => { }, isRunning);

        var binaries = source
            .WhereNotEmpty()
            .Select(assembler.Assemble)
            .WhereSuccess()
            .Select(x => x.ProgramBinary);

        var canRun = source
            .WhereNotEmpty()
            .Select(assembler.Assemble)
            .IsSuccess();

        Run = ReactiveCommand.CreateFromObservable(() => RunFirstBinaryUntilHalted(binaries), canRun);
        Run.IsExecuting.Subscribe(isRunning);
    }

    private IObservable<Status> RunFirstBinaryUntilHalted(IObservable<byte[]> assemblies)
    {
        return assemblies
            .Take(1)
            .SelectMany(bytes => Z80.RunUntilHalted(bytes, NewThreadScheduler.Default, Stop));
    }

    public ReactiveCommand<Unit, Unit> Stop { get; }

    public ReactiveCommand<Unit, Status> Run { get; }
}