using System.Reactive;
using System.Reactive.Disposables;
using EagleEye.Apps.Models;
using ReactiveUI;
using WebViewControl;

namespace EagleEye.Apps.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor
        
        public MainViewModel()
        {
            this.WhenActivated(disposables =>
            {
                Disposable
                    .Create(() => {})
                    .DisposeWith(disposables);
            });
        }
        
        #endregion
        
        #region Methods

        public ReactiveCommand<WebView,Unit> OnWebViewReady = ReactiveCommand.Create<WebView>(
            webView =>
            {
                webView.ShowDeveloperTools();
                webView.RegisterJavascriptObject("MyAndroid", new PlatformFeature());
            });
        
        #endregion
    }
}