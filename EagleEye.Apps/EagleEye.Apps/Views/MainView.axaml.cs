using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using EagleEye.Apps.ViewModels;
using ReactiveUI;
using WebViewControl;

namespace EagleEye.Apps.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    #region Constructor

    public MainView()
    {
        this.WhenActivated(disposables =>
        {
            this.Events().Initialized
                .Select(_ => this as ContentControl)
                .InvokeCommand(ViewModel!.OnViewReady)
                .DisposeWith(disposables);
        });
        InitializeComponent();
    }

    #endregion
}