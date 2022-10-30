using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(new Z80AdapterAssembler())
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
