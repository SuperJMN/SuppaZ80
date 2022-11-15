using System;
using System.Collections.Generic;
using System.Globalization;
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

        MemoryBlockLists = GroupMemoryBlocks(GetMemory(Debugger.StatusChanged));
        RegisterLists = GetRegisters(Debugger.StatusChanged);

        Source = "; This sample adds 2 numbers\r\n\r\n\tCALL MAIN\r\n\tHALT\r\nMAIN:\r\n\tLD a, 1\r\n\tLD b, 2\r\n\tADD a, b\r\n\tRET";
    }

    public IObservable<List<NamedMemory>> RegisterLists { get; }

    private IObservable<List<NamedMemory>> GetRegisters(IObservable<Status> debuggerStatusChanged)
    {
        return debuggerStatusChanged.Select(x => x.RawRegisters).Select(x =>
        {
            return new[]
            {
                NamedMemory(x.AF, nameof(x.AF)),
                NamedMemory(x.BC, nameof(x.BC)),
                NamedMemory(x.DE, nameof(x.DE)),
                NamedMemory(x.HL, nameof(x.HL)),
                NamedMemory(x.PC, nameof(x.PC)),
                NamedMemory(x.SP, nameof(x.SP))
            }.ToList();
        });
    }

    private NamedMemory NamedMemory(short value, string name)
    {
        return new NamedMemory(name, new ModifiableValue<short>(value, text => short.Parse(text, NumberStyles.HexNumber), x => $"{x:X4}", x => Debugger.SetRegister(name, x)));
    }

    private NamedMemory NamedMemory(ushort value, string name)
    {
        return new NamedMemory(name, new ModifiableValue<ushort>(value, text => ushort.Parse(text, NumberStyles.HexNumber), x => $"{x:X4}", x => Debugger.SetRegister(name, x)));
    }

    private static IObservable<IEnumerable<MemoryBlockViewModel>> GroupMemoryBlocks(IObservable<IEnumerable<IndexedMemory>> memoryVms)
    {
        return memoryVms.Select(a => a.Chunk(4)).Select(b => b.Select((m, i) => new MemoryBlockViewModel(m, i * 4)));
    }

    private IObservable<IEnumerable<IndexedMemory>> GetMemory(IObservable<Status> processorStatus)
    {
        return processorStatus
            .Select(status => status.RawMemory.Take(32))
            .Select(bytes => bytes.Select((b, i) => new IndexedMemory(i, new ModifiableValue<byte>(b, s => byte.Parse(s, NumberStyles.HexNumber), x => $"{x:X2}", x => Debugger.SetMemory(i, x)))));
    }

    public IObservable<IEnumerable<MemoryBlockViewModel>> MemoryBlockLists { get; }

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