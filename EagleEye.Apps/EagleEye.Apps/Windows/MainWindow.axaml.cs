using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using EagleEye.Apps.ViewModels;

namespace EagleEye.Apps.Windows
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}