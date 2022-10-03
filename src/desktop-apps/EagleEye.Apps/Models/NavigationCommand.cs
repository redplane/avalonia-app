using EagleEye.Apps.Models.MessageEvents;
using ReactiveUI;

namespace EagleEye.Apps.Models;

public class NavigationCommand
{
    #region Properties

    public RoutingState Router { get; }
    
    public IRoutableViewModel ViewModel { get; }
    
    #endregion
    
    #region Constructor

    public NavigationCommand(RoutingState router, IRoutableViewModel viewModel)
    {
        Router = router;
        ViewModel = viewModel;
    }

    #endregion
}