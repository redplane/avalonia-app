using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using EagleEye.Apps.Models;
using EagleEye.Apps.Models.MessageEvents;
using EagleEye.Apps.Views;
using EagleEye.Apps.Windows;
using LiteMessageBus.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps.ViewModels;

public class MainWindowViewModel : ReactiveObject, IActivatableViewModel, IScreen
{
    #region Accessors

    public RoutingState Router { get; set; }

    public ViewModelActivator Activator { get; } = new();

    #endregion

    #region Constructor

    public MainWindowViewModel()
    {
        Router = new RoutingState();

        // Router.NavigateAndReset.Execute(splashViewModel);
        this.WhenActivated(disposables =>
        {
            Router.NavigateAndReset.Execute(new SplashViewModel(this));
            Disposable
                .Create(() =>
                {
                    
                    
                })
                .DisposeWith(disposables);
        });
        
    }

    #endregion

    #region Methods

    private readonly ReactiveCommand<NavigationCommand, Unit> _navigate = ReactiveCommand.Create<NavigationCommand>(message =>
    {
        message.Router.Navigate.Execute(message.ViewModel);
    });

    #endregion
}