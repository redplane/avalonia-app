using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EagleEye.Apps.Views;

public partial class SplashView : UserControl
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