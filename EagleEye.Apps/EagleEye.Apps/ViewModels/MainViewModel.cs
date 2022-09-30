using System.Reactive;
using System.Reactive.Disposables;
using EagleEye.Apps.Models;
using ReactiveUI;
using Splat;
using WebViewControl;

namespace EagleEye.Apps.ViewModels
{
    public class MainViewModel : ViewModelBase, IRoutableViewModel
    {
        #region Properties

        public string? UrlPathSegment => "main";
        
        public IScreen HostScreen { get; }
        
        #endregion
        
        #region Constructor
        
        public MainViewModel(IScreen screen)
        {
            this.WhenActivated(disposables =>
            {
                Disposable
                    .Create(() => {})
                    .DisposeWith(disposables);
            });

            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }
        
        #endregion
        
        #region Methods

        public ReactiveCommand<WebView,Unit> OnWebViewReady = ReactiveCommand.Create<WebView>(
            webView =>
            {
                webView.RegisterJavascriptObject("MyAndroid", new PlatformFeature());
            });
        
        #endregion
    }
}