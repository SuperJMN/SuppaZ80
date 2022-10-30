using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        Open = helper.Open;

        Open.SelectMany(r => r.Match(LoadFile, _ => Task.FromResult("")))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(s => Source = s)
            .Subscribe();

        var assembledResult = this.WhenAnyValue(x => x.Source)
            .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
            .Where(s => s.Trim() != "")
            .Select(Assemble)
            .Publish()
            .RefCount();

        Errors = assembledResult.Select(x => x.Match(_ => "", s => s));

        var program = assembledResult.Where(x => x.IsSuccess).Select(x => x.Value);
        Processor = new Z80ViewModel(assembler, this.WhenAnyValue(x => x.Source));
        StepProcessor = new Z80StepViewModel(program);

        Memory = Processor.Run.Merge(StepProcessor.Step).Select(x => x.Memory.Take(128));
        Registers = Processor.Run.Merge(StepProcessor.Step).Select(x => x.Registers);

        Source = "; This sample adds 2 numbers\r\n\r\n\tCALL MAIN\r\n\tHALT\r\nMAIN:\r\n\tLD a, 1\r\n\tLD b, 2\r\n\tADD a, b\r\n\tRET";
    }

    public IObservable<string> Errors { get; }

    [Reactive] public string Source { get; set; } = "";

    public IObservable<List<RegisterViewModel>> Registers { get; }

    public IObservable<IEnumerable<MemoryViewModel>> Memory { get; }

    public IZ80StepViewModel StepProcessor { get; }

    public IZ80ViewModel Processor { get; }

    public ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }

    private static async Task<string> LoadFile(IZafiroFile file)
    {
        await using var s = await file.OpenRead();
        return await s.ReadToEnd();
    }

    private Result<byte[]> Assemble(string code)
    {
        return assembler.Assemble(code);
    }
}