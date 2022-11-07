using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using SuppaZ80.Models;
using Zafiro.FileSystem;

namespace SuppaZ80.ViewModels.Design;

public class MainViewModelDesign : IMainViewModel
{
    public IDebugger Debugger => new DebuggerDesign();
    public IZ80ViewModel Processor => new Z80ViewModelDesign();

    public IObservable<Registers> Registers => Observable
        .Return(new Registers(new Register[] { new("Fake", 1) }));

    public IObservable<IEnumerable<MemoryCell>> Memory => Observable
        .Return(new List<MemoryCell> { new(123, 1) });

    public IObservable<string> Errors => Observable.Never("");

    public string Source { get; set; } =
        "?//-----------------------------------------------------------------------------\r\n// Copyright (c) 2017-2022 informedcitizenry <informedcitizenry@gmail.com>\r\n//\r\n// Licensed under the MIT license. See LICENSE for full license information.\r\n// \r\n//-----------------------------------------------------------------------------\r\n        .cpu \"z80\"\r\nputc    =  $bb5a\r\n        * = 1200\r\n        ld hl,message\r\n        and a,a\r\n-       ld a,(hl)\r\n        rrca\r\n        push af\r\n        call putc\r\n        pop af\r\n        ret c\r\n        inc hl\r\n        jr -\r\nmessage .lstring \"Hello, World!\"";

    public ReactiveCommand<Unit, Result<IZafiroFile>> Open => ReactiveCommand.Create(() => new Result<IZafiroFile>());

    public IObservable<IEnumerable<MemoryBlockViewModel>> MemoryBlocks => Observable.Return(new List<MemoryBlockViewModel>
    {
        new(new List<MemoryViewModel> { new(new MemoryCell(0, 122), _ => { }) }, 0),
        new(new List<MemoryViewModel> { new(new MemoryCell(1, 122), _ => { }) }, 3)
    });

    public IObservable<IEnumerable<MemoryViewModel>> MemoryVms { get; }
}