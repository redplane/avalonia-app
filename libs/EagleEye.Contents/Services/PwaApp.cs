using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using EagleEye.Contents.Constants;
using EagleEye.Contents.Models;
using Microsoft.AspNetCore.Hosting;
using WebViewControl;

namespace EagleEye.Contents.Services
{
    public class PwaApp : IDisposable
    {
        #region Properties

        private readonly Func<Task<GetNextPwaVersionResult>> _checkNextUpdateAsyncCallback;

        private readonly Func<DownloadPwaContentRequest, Task<byte[]>> _downloadUpdateAsyncCallback;

        private readonly string _webContentFolder = Folders.WebContent;

        private readonly string _temporaryFolder = Folders.TemporaryFolder;

        private CancellationTokenSource _cancellationTokenSource;

        #endregion
        
        #region Constructor

        public PwaApp(Func<Task<GetNextPwaVersionResult>> checkNextUpdateAsyncCallback = null, 
            Func<DownloadPwaContentRequest, Task<byte[]>> downloadUpdateAsyncCallback = null)
        {
            _checkNextUpdateAsyncCallback = checkNextUpdateAsyncCallback;
            _downloadUpdateAsyncCallback = downloadUpdateAsyncCallback;
        }

        #endregion
        
        #region Methods

        public void Show(ContentControl contentControl)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            Task.Run(async () =>
            {
                // Create a folder to store web content.
                var absoluteWebContentDirectory = MakeContentDirectory();
                var temporaryDirectory = MakeTemporaryDirectory();

                if (_checkNextUpdateAsyncCallback != null)
                {
                    var nextUpdate = await _checkNextUpdateAsyncCallback();
                    if (nextUpdate != null && _downloadUpdateAsyncCallback != null)
                    {
                        var downloadRequest = new DownloadPwaContentRequest();
                        downloadRequest.Version = nextUpdate.Version;
                        downloadRequest.DownloadUrl = nextUpdate.DownloadUrl;

                        var downloadedContent = await _downloadUpdateAsyncCallback(downloadRequest);
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

                // Start the kestrel server.
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(absoluteWebContentDirectory)
                    .UseStartup<Startup>()
                    .UseUrls("http://0.0.0.0:5005")
                    .Build();
                
                await host.RunAsync();
            }, _cancellationTokenSource.Token);
            
            var webView = new WebView();
            contentControl.Content = webView;
            webView.LoadUrl("http://localhost:5005");
            webView.ShowDeveloperTools();
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

            foreach(var fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch(Exception) { } // Ignore all exceptions
            }

            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                try
                {
                    di.Delete();
                }
                catch(Exception) { } // Ignore all exceptions
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
        
        #endregion

        
    }
}