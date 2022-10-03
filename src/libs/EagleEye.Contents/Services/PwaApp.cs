using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using EagleEye.Contents.Constants;
using EagleEye.Contents.Handlers;
using EagleEye.Contents.Interfaces;
using EagleEye.Contents.Methods;
using EagleEye.Contents.Models;
using EagleEye.Contents.Models.PwaOptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebViewControl;

namespace EagleEye.Contents.Services
{
    public class PwaApp : IDisposable
    {
        #region Properties

        private readonly string _webContentFolder = Folders.WebContent;

        private readonly string _temporaryFolder = Folders.TemporaryFolder;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly PwaOption _options;

        #endregion

        #region Constructor

        public PwaApp(PwaOption options)
        {
            _options = options;
        }

        #endregion

        #region Methods

        public void Show(ContentControl contentControl,
            Type[] nativeMethodTypes = null)
        {
            var webView = new WebView();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var services = new ServiceCollection();
            if (nativeMethodTypes != null)
            {
                foreach (var nativeMethod in nativeMethodTypes)
                    services.AddScoped(typeof(INativeMethod), nativeMethod);
            }
            
            // Built in native method.
            services.AddScoped<INativeMethod, LocalPushNativeMethod>();
            services.AddSingleton(webView);

            services.AddSingleton(AvaloniaLocator.Current.GetService<DesktopNotifications.INotificationManager>()!);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            var endPoint = string.Empty;

            Task.Run(async () =>
            {
                // Create a folder to store web content.
                var absoluteWebContentDirectory = MakeContentDirectory();
                var temporaryDirectory = MakeTemporaryDirectory();

                if (_options is OfflinePwaOption offlinePwaOption)
                {
                    var checkNextUpdateAsyncCallback = offlinePwaOption.CheckNextUpdateHandler;
                    var downloadUpdateAsyncCallback = offlinePwaOption.DownloadUpdateHandler;

                    if (checkNextUpdateAsyncCallback != null)
                    {
                        var nextUpdate = await checkNextUpdateAsyncCallback();
                        if (nextUpdate != null)
                        {
                            var downloadRequest = new DownloadPwaContentRequest();
                            downloadRequest.Version = nextUpdate.Version;
                            downloadRequest.DownloadUrl = nextUpdate.DownloadUrl;

                            var downloadedContent = await downloadUpdateAsyncCallback(downloadRequest);
                            if (downloadedContent == null)
                                throw new Exception("Something went wrong while downloading the PWA content update");

                            // Delete every thing before extracting.
                            ClearFolder(absoluteWebContentDirectory);

                            // Save the downloaded content to folder.
                            var downloadedZipFile = await SaveAsAsync(temporaryDirectory, downloadedContent);

                            var extractPath = absoluteWebContentDirectory;
                            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                                extractPath += Path.DirectorySeparatorChar;

                            // Extract the zip file to the web content folder.
                            ZipFile.ExtractToDirectory(downloadedZipFile, extractPath);

                            // Delete the temporary file.
                            File.Delete(downloadedZipFile);
                        }
                    }

                    // Get a free port
                    var port = GetAvailablePort(1024);
                    endPoint = $"http://localhost:{port}";

                    // Start the kestrel server.
                    var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(absoluteWebContentDirectory)
                        .UseStartup<Startup>()
                        .UseUrls(endPoint)
                        .Build();

                    host.RunAsync(_cancellationTokenSource.Token);
                }
                else if (_options is OnlinePwaOption onlinePwaOption)
                {
                    endPoint = onlinePwaOption.Endpoint;
                }

                webView.RegisterJavascriptObject("Android", new MethodExecutor(serviceProvider));
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    contentControl.Content = webView;
                    webView.LoadUrl(endPoint);
                    webView.ShowDeveloperTools();
                });
                
            }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        #endregion

        #region Internal methods

        private string MakeContentDirectory()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(currentDirectory))
                throw new Exception("Cannot create web content directory.");

            var webContentDirectory = Path.Combine(currentDirectory, _webContentFolder);
            if (!Directory.Exists(webContentDirectory))
                Directory.CreateDirectory(webContentDirectory);

            return webContentDirectory;
        }

        private string MakeTemporaryDirectory()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(currentDirectory))
                throw new Exception("Cannot temporary directory.");

            var temporaryDirectory = Path.Combine(currentDirectory, _temporaryFolder);
            if (!Directory.Exists(temporaryDirectory))
                Directory.CreateDirectory(temporaryDirectory);

            return temporaryDirectory;
        }

        private void ClearFolder(string folderName)
        {
            var dir = new DirectoryInfo(folderName);

            foreach (var fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception)
                {
                } // Ignore all exceptions
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                try
                {
                    di.Delete();
                }
                catch (Exception)
                {
                } // Ignore all exceptions
            }
        }

        private async Task<string> SaveAsAsync(string designatedFolder, byte[] content)
        {
            var fileName = Guid.NewGuid().ToString("N");
            var designatedFileName = Path.Combine(designatedFolder, fileName);
            await using var stream = File.Create(designatedFileName);
            await stream.WriteAsync(content, 0, content.Length);
            await stream.FlushAsync();

            return designatedFileName;
        }

        private int GetAvailablePort(int startingPort)
        {
            var portArray = new List<int>();

            var properties = IPGlobalProperties.GetIPGlobalProperties();

            // Ignore active connections
            var connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                where n.LocalEndPoint.Port >= startingPort
                select n.LocalEndPoint.Port);

            // Ignore active tcp listners
            var endPoints = properties.GetActiveTcpListeners();
            portArray.AddRange(from n in endPoints
                where n.Port >= startingPort
                select n.Port);

            // Ignore active UDP listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                where n.Port >= startingPort
                select n.Port);

            portArray.Sort();

            for (var i = startingPort; i < UInt16.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }

        #endregion
    }
}