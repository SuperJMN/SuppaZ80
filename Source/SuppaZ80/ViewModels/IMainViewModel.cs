using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace SuppaZ80.ViewModels;

public interface IMainViewModel
{
    IDebugger Debugger { get; }
    IZ80ViewModel Processor { get; }
    IObservable<Registers> Registers { get; }
    IObservable<IEnumerable<MemoryViewModel>> Memory { get; }
    IObservable<string> Errors { get; }
    string Source { get; set; }
    ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }
}