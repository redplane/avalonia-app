using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using EagleEye.Apps.ViewModels;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps.Windows
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private RoutedViewHost _viewHost => this.FindControl<RoutedViewHost>("RoutedViewHost");
        
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            
            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
                
                Disposable
                    .Create(() =>
                    {
                        _viewHost.Events().Initialized
                            .InvokeCommand(ViewModel.OnViewReady);
                    })
                    .DisposeWith(disposables);
            });
        }
    }
}