using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps.ViewModels;

public class SplashViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    #region Accessors

    public ViewModelActivator Activator { get; } = new();

    public string? UrlPathSegment => "splash";
    
    public IScreen HostScreen { get; }
    
    #endregion

    #region Constructor

    public SplashViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() =>
                {
                    /* Handle deactivation */
                })
                .DisposeWith(disposables);
        });
    }

    #endregion
}