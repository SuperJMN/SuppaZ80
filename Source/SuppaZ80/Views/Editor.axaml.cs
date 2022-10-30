using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Indentation;

namespace SuppaZ80.Views;

public partial class Editor : UserControl
{
    public static readonly StyledProperty<string> CodeProperty = AvaloniaProperty.Register<Editor, string>(
        "Code");

    public Editor()
    {
        InitializeComponent();

        var textEditor = this.FindControl<TextEditor>("textCode");
        textEditor.ShowLineNumbers = true;
        textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
    }

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }
}