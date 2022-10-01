using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using EagleEye.Apps.Models;
using EagleEye.Apps.Models.Pwas;
using EagleEye.Apps.Services.Abstractions;
using EagleEye.Contents.Models;
using EagleEye.Contents.Services;
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

        public MainViewModel()
        {
            this.WhenActivated(disposables =>
            {
                Disposable
                    .Create(() => { })
                    .DisposeWith(disposables);
            });

            HostScreen = Locator.Current.GetService<IScreen>();
        }

        #endregion

        #region Methods

        public readonly ReactiveCommand<ContentControl, Unit> OnViewReady = ReactiveCommand.Create<ContentControl>(
            contentControl =>
            {
                var hasUpdateAsyncHandler = new Func<Task<GetNextPwaVersionResult>>(async () =>
                {
                    var configurationService = Locator.Current.GetService<IConfigurationService>();
                    var pwaService = Locator.Current.GetService<IPwaService>();

                    var configuration = await configurationService.GetAsync();
                    var request = new GetNextPwaVersionRequest();
                    request.Environment = "Development";
                    request.Version = configuration.Version;

                    var loadNextVersionResult = await pwaService.GetVersionAsync(request);
                    if (loadNextVersionResult != null)
                        return loadNextVersionResult;

                    return null;
                });
                
                var downloadUpdateAsyncHandler = new Func<DownloadPwaContentRequest, Task<byte[]>>(async request =>
                {
                    var configurationService = Locator.Current.GetService<IConfigurationService>();
                    var pwaService = Locator.Current.GetService<IPwaService>();
                    var content = await pwaService.DownloadAsync(request.DownloadUrl);

                    await configurationService.UpdateAsync(configuration =>
                    {
                        configuration.Version = request.Version;
                        return configuration;
                    });
                    
                    return content;
                });
                
                var pwaApp = new PwaApp(hasUpdateAsyncHandler, downloadUpdateAsyncHandler);
                pwaApp.Show(contentControl);
            });

        #endregion
    }
}