using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro.FileSystem;

namespace SuppaZ80.Models;

public class ZafiroHelpers
{
    public ZafiroHelpers()
    {
        IZafiroFileSystem fs = new ZafiroFileSystem(new FileSystem(), Maybe<ILogger>.None);

        var files = Observable
            .FromAsync(PickFile)
            .Select(path => path.Map(s => fs.GetFile(s)))
            .Where(maybe => maybe.HasValue)
            .Select(x => x.Value);

        Open = ReactiveCommand.CreateFromObservable(() => files);
    }

    public ReactiveCommand<Unit, Result<IZafiroFile>> Open { get; }

    private static async Task<Maybe<string>> PickFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter> { new() { Extensions = new List<string> { "z80" }, Name = "Z80 assembly" } }
        };

        var mainWindow = ((ClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime).MainWindow;
        var showAsync = await openFileDialog.ShowAsync(mainWindow);

        var firstOrDefault = showAsync?.FirstOrDefault();
        return Maybe.From(firstOrDefault);
    }
}