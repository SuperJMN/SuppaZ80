namespace SuppaZ80.ViewModels;

public class NamedMemory
{
    public string Name { get; }
    public IText Value { get; }

    public NamedMemory(string name, IText value)
    {
        Name = name;
        Value = value;
    }
}