using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using SuppaZ80.Models;
using Zafiro.FileSystem;

namespace SuppaZ80.ViewModels;

public interface IMainViewModel
{
    IDebugger Debugger { get; }
    IZ80ViewModel Processor { get; }
    IObservable<Registers> Registers { get; }
    IObservable<string> Errors { get; }
    string Source { get; set; }
    ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }
    IObservable<IEnumerable<MemoryBlockViewModel>> MemoryBlocks { get; }
}