using System;

namespace SuppaZ80.ViewModels;

public class Register : RegisterViewModel
{
    public Register(string name, int value)
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