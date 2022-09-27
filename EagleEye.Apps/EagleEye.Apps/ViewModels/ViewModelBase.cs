using ReactiveUI;

namespace EagleEye.Apps.ViewModels;

public class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    #region Accessors
    
    public ViewModelActivator Activator { get; } = new();
    
    #endregion
}