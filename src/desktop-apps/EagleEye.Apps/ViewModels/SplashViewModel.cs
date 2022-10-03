using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EagleEye.Apps.Constants;
using EagleEye.Apps.Models.MessageEvents;
using LiteMessageBus.Services.Interfaces;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps.ViewModels;

public class SplashViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    #region Accessors

    public ViewModelActivator Activator { get; } = new();

    public string? UrlPathSegment => "splash";
    
    public IScreen HostScreen { get; }

    public readonly ReactiveCommand<Unit, Unit> OnViewInitialized = ReactiveCommand.CreateFromObservable(
        () => Observable.Start(() => { }, TaskPoolScheduler.Default)
            .Delay(TimeSpan.FromSeconds(3))
            .Do(_ =>
            {
                var messageBusService = Locator.Current.GetService<IRxMessageBus>();
                messageBusService.AddMessage<string>(MessageChannelNames.MainWindow, NavigationMessageEvents.Navigation, ScreenCodes.Main);
            })
        );

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