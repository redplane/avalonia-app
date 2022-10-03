using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EagleEye.Apps.Models;
using EagleEye.Apps.Services.Abstractions;
using EagleEye.Apps.Services.Implementations;
using EagleEye.Apps.ViewModels;
using EagleEye.Apps.Views;
using EagleEye.Apps.Windows;
using LiteMessageBus.Services.Implementations;
using LiteMessageBus.Services.Interfaces;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(ViewLocator).Assembly);
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindowViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<IRxMessageBus>(() => new InMemoryRxMessageService());
            Locator.CurrentMutable.RegisterConstant<IConfigurationService>(new ConfigurationService());
            Locator.CurrentMutable.Register<IPwaService>(() => new PwaService());

            var mainWindowViewModel = Locator.Current.GetService<MainWindowViewModel>();
            Locator.CurrentMutable.Register<IScreen>(() => mainWindowViewModel);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewForMainWindow = Locator.Current.GetService<IViewFor<MainWindowViewModel>>();
                if (viewForMainWindow is MainWindow mainWindow)
                    desktop.MainWindow = mainWindow;
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                var mainView = Locator.Current.GetService<MainView>();
                singleViewPlatform.MainView = mainView;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}