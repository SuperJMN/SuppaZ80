using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Indentation;
using CSharpFunctionalExtensions;

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

    public static readonly StyledProperty<Maybe<int>> CurrentLineProperty = AvaloniaProperty.Register<Editor, Maybe<int>>(
        "CurrentLine");

    public Maybe<int> CurrentLine
    {
        get => GetValue(CurrentLineProperty);
        set => SetValue(CurrentLineProperty, value);
    }

    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<Editor, bool>(
        "IsReadOnly");

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
}