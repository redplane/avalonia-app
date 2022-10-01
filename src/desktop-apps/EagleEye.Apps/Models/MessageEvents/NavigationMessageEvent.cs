using EagleEye.Apps.Constants;
using LiteMessageBus.Models;
using ReactiveUI;

namespace EagleEye.Apps.Models.MessageEvents;

public class NavigationMessageEvent : TypedChannelEvent<IRoutableViewModel>
{
    #region Properties

    public IRoutableViewModel ViewModel { get; set; }

    #endregion

    #region Methods

    public NavigationMessageEvent()
        : base(MessageChannelNames.MainWindow, NavigationMessageEvents.Navigation)
    {
    }

    #endregion
}