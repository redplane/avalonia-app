using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using EagleEye.Apps.ViewModels;
using ReactiveUI;

namespace EagleEye.Apps.Views;

public partial class SplashView : ReactiveUserControl<SplashViewModel>
{
    #region Constructor
    
    public SplashView()
    {
        this.WhenActivated(disposables =>
        {
            this.Events().Initialized
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel!.OnViewInitialized)
                .DisposeWith(disposables);
        });
        
        AvaloniaXamlLoader.Load(this);
    }

    #endregion
}