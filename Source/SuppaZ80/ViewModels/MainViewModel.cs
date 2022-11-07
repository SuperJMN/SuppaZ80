using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sixty502DotNet;
using SuppaZ80.Models;
using Zafiro.Core.Mixins;
using Zafiro.FileSystem;

namespace SuppaZ80.ViewModels;

public class MainViewModel : ViewModelBase, IMainViewModel
{
    private readonly IAssembler assembler;

    public MainViewModel(IAssembler assembler)
    {
        this.assembler = assembler;
        var helper = new ZafiroHelpers();

        var assembledResult = this.WhenAnyValue(x => x.Source)
            .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
            .Where(s => s.Trim() != "")
            .Select(Assemble)
            .Publish()
            .RefCount();

        Errors = assembledResult.Select(x => x.Match(_ => "", s => s));

        Processor = new Z80ViewModel(assembler, this.WhenAnyValue(x => x.Source));
        Debugger = new Debugger(assembledResult);

        Open = ReactiveCommand.CreateFromObservable(() => helper.Files, Debugger.IsDebugging.Not());
        Open.SelectMany(r => r.Match(LoadFile, _ => Task.FromResult("")))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(s => Source = s)
            .Subscribe();

        MemoryBlocks = GroupMemoryBlocks(GetMemory(Debugger.Status));
        Registers = Processor.Run.Merge(Debugger.Status).Select(x => x.Registers);

        Source = "; This sample adds 2 numbers\r\n\r\n\tCALL MAIN\r\n\tHALT\r\nMAIN:\r\n\tLD a, 1\r\n\tLD b, 2\r\n\tADD a, b\r\n\tRET";
    }

    private static IObservable<IEnumerable<MemoryBlockViewModel>> GroupMemoryBlocks(IObservable<IEnumerable<MemoryViewModel>> memoryVms)
    {
        return memoryVms.Select(a => a.Chunk(4)).Select(b => b.Select((m, i) => new MemoryBlockViewModel(m, i * 4)));
    }

    private IObservable<IEnumerable<MemoryViewModel>> GetMemory(IObservable<ProcessorStatus> processorStatus)
    {
        return processorStatus
            .Select(status => status.Memory.Take(32))
            .Select(x => x.Select(cell => new MemoryViewModel(cell, v => Debugger.SetMemory(cell.Location, v))));
    }

    public IObservable<IEnumerable<MemoryBlockViewModel>> MemoryBlocks { get; }

    public IObservable<string> Errors { get; }

    [Reactive] public string Source { get; set; } = "";

    public IObservable<Registers> Registers { get; }

    public IDebugger Debugger { get; }

    public IZ80ViewModel Processor { get; }

    public ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }

    private static async Task<string> LoadFile(IZafiroFile file)
    {
        await using var s = await file.OpenRead();
        return await s.ReadToEnd();
    }

    private Result<AssemblyData> Assemble(string code)
    {
        return assembler.Assemble(code);
    }
}