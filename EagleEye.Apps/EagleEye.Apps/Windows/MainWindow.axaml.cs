using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using EagleEye.Apps.Constants;
using EagleEye.Apps.Models.MessageEvents;
using EagleEye.Apps.ViewModels;
using LiteMessageBus.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;

namespace EagleEye.Apps.Windows
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        #region Properties
        
        private readonly LinkedList<IDisposable> _subscriptions;
        
        private RoutedViewHost _viewHost => this.FindControl<RoutedViewHost>("RoutedViewHost");
        
        #endregion
        
        #region Constructor
        
        public MainWindow()
        {
            _subscriptions = new LinkedList<IDisposable>();
            
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            
            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);
            });
            
            InitializeComponent();
        }
        
        #endregion
        
        #region Internal methods

        protected override void OnInitialized()
        {
            _subscriptions.Clear();
            
            base.OnInitialized();

            var messageBus = Locator.Current.GetService<IRxMessageBus>()!;
            var subscription = messageBus.HookMessageChannel<string>(MessageChannelNames.MainWindow, 
                    NavigationMessageEvents.Navigation)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(async screenCode =>
                {
                    RxApp.MainThreadScheduler.Schedule(() =>
                    {
                        if (screenCode == ScreenCodes.Main)
                            _viewHost.Router.Navigate.Execute(new MainViewModel());
                    });

                });
            _subscriptions.AddLast(subscription);
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();
            
            base.OnClosed(e);
        }
        
        #endregion
    }
}