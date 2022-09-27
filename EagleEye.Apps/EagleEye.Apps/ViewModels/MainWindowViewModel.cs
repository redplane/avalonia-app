using System.Reactive.Disposables;
using ReactiveUI;

namespace EagleEye.Apps.ViewModels;

public class MainWindowViewModel : ReactiveObject, IActivatableViewModel
{
    #region Accessors

    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    #endregion
    
    #region Constructor

    public MainWindowViewModel()
    {
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(() => { /* Handle deactivation */ })
                .DisposeWith(disposables);
        });
    }
    
    #endregion
}