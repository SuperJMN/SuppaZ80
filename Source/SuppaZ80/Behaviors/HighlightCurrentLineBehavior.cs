using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Rendering;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace SuppaZ80.Behaviors;

public class HighlightCurrentLineBehavior : Behavior<TextEditor>
{
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(new Renderer(this.GetObservable(LineNumberProperty)));
        this.WhenAnyValue(x => x.LineNumber)
            .Do(_ => AssociatedObject.TextArea.TextView.InvalidateVisual())
            .Subscribe();
    }

    private class Renderer : IBackgroundRenderer
    {
        private readonly BehaviorSubject<Maybe<int>> sub;
        private readonly SolidColorBrush solidColorBrush;

        public Renderer(IObservable<Maybe<int>> lineNumber)
        {
            sub = new BehaviorSubject<Maybe<int>>(Maybe<int>.None);
            lineNumber.Subscribe(sub);
            solidColorBrush = new SolidColorBrush(Color.Parse("#F2E371"));
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            sub.Value.Execute(i => Draw(textView, i, drawingContext));
        }

        private void Draw(TextView textView, int lineNumber, DrawingContext drawingContext)
        {
            var currentLine = textView.Document.GetLineByNumber(lineNumber);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(solidColorBrush, null, new Rect(rect.Position, new Size(textView.Bounds.Width, rect.Height)));
            }
        }

        public KnownLayer Layer => KnownLayer.Background;
    }

    public static readonly StyledProperty<Maybe<int>> LineNumberProperty = AvaloniaProperty.Register<HighlightCurrentLineBehavior, Maybe<int>>(
        "LineNumber");

    public Maybe<int> LineNumber
    {
        get => GetValue(LineNumberProperty);
        set => SetValue(LineNumberProperty, value);
    }
}