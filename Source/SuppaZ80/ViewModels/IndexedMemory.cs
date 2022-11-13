namespace SuppaZ80.ViewModels;

public class IndexedMemory
{
    public int Id { get; }
    public IText Value { get; }

    public IndexedMemory(int id, IText value)
    {
        Id = id;
        Value = value;
    }
}