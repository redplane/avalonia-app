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
            _webView.Events().Initialized
                .Select(_ => this._webView)
                .InvokeCommand(ViewModel!.OnWebViewReady)
                .DisposeWith(disposables);
        });
        InitializeComponent();
    }

    #endregion

    #region Properties

    private WebView _webView => this.FindControl<WebView>("webView");

    #endregion
}