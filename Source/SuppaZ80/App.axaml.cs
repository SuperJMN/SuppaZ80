using System.Xml;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using SuppaZ80.Models;
using SuppaZ80.ViewModels;
using SuppaZ80.Views;

namespace SuppaZ80
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            RegisterHighligthing();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(new Z80AdapterAssembler())
                };
            }


            base.OnFrameworkInitializationCompleted();
        }

        private static void RegisterHighligthing()
        {
            using var stream = typeof(App).Assembly.GetManifestResourceStream("SuppaZ80.Highlighting.Z80ASM.xml");
            using var reader = new XmlTextReader(stream!);
            HighlightingManager.Instance.RegisterHighlighting("Z80", new string[0], HighlightingLoader.Load(reader, HighlightingManager.Instance));
        }
    }
}
