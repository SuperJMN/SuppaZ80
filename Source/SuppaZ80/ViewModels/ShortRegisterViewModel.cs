using System;

namespace SuppaZ80.ViewModels;

public class ShortRegisterViewModel : RegisterViewModel
{
    public ShortRegisterViewModel(string name, short value)
    {
        Name = name;
        Value = value;
        var bytes = BitConverter.GetBytes(value);
        Hi = bytes[0];
        Lo = bytes[1];
    }

    public int Value { get; }

    public byte Lo { get; }

    public byte Hi { get; }
}