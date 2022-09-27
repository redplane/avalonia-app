using System.Reactive.Disposables;
using ReactiveUI;

namespace EagleEye.Apps.ViewModels;

public class SplashViewModel : ReactiveObject, IActivatableViewModel
{
    #region Accessors

    public ViewModelActivator Activator { get; } = new();

    #endregion

    #region Constructor

    public SplashViewModel()
    {
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