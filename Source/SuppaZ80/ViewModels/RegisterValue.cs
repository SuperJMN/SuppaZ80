using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace SuppaZ80.ViewModels;

public interface IText
{
    public string Text { get; }
}

public class ModifiableValue<T> : ModifiableValue, IText where T : IConvertible
{
    public ModifiableValue(T value, Func<string, T> parse, Func<T, string> unparse, Action<T> setValue)
    {
        Text = unparse(value);

        Update = ReactiveCommand.Create(() =>
        {
            var convertible = parse(Text);
            setValue(convertible);
            return convertible;
        }, this.IsValid());

        Update
            .Select(unparse)
            .BindTo(this, x => x.Text);

        this.ValidationRule(x => x.Text, s =>
        {
            var isValid = int.TryParse(s, NumberStyles.HexNumber, CultureInfo.CurrentUICulture, out _);
            return isValid;
        }, "Invalid value");
    }

    public ReactiveCommand<Unit, T> Update { get; }
}

public static class Mixin
{
    public static T ChangeType<T>(this object obj)
    {
        return (T) Convert.ChangeType(obj, typeof(T));
    }
}