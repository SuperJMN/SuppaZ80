using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace SuppaZ80.ViewModels;

public interface IMainViewModel
{
    IZ80StepViewModel StepProcessor { get; }
    IZ80ViewModel Processor { get; }
    IObservable<List<RegisterViewModel>> Registers { get; }
    IObservable<IEnumerable<MemoryViewModel>> Memory { get; }
    IObservable<string> Errors { get; }
    string Source { get; set; }
    ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }
}