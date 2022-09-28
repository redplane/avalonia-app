using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using EagleEye.Apps.ViewModels;

namespace EagleEye.Apps.Views;

public partial class SplashView : ReactiveUserControl<SplashViewModel>
{
    #region Constructor
    
    public SplashView()
    {
        InitializeComponent();
    }

    #endregion
    
    #region Methods
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    #endregion
}