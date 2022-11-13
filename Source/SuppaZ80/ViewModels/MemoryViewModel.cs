using System;
using System.Globalization;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using SuppaZ80.Models;

namespace SuppaZ80.ViewModels;

public class MemoryViewModel : ReactiveValidationObject
{
    public MemoryViewModel(MemoryCell cell, Action<byte> setValue)
    {
        Update = ReactiveCommand.Create(() => setValue(Parse(Text)), this.IsValid());

        Text = cell.Value.ToString("X2");
        this.ValidationRule(x => x.Text, s =>
        {
            var isValid = int.TryParse(s, NumberStyles.HexNumber, CultureInfo.CurrentUICulture, out _);
            return isValid;
        }, "Invalid value");
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    private byte Parse(string text)
    {
        return byte.Parse(text, NumberStyles.HexNumber);
    }

    [Reactive] public string Text { get; set; }
}