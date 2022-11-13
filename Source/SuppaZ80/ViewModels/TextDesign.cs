namespace SuppaZ80.ViewModels;

internal class TextDesign : IText
{
    public TextDesign(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override string ToString()
    {
        return Text;
    }
}