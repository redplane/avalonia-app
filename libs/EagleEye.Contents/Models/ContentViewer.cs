using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using WebViewControl;

namespace EagleEye.Contents.Models
{
    public class ContentViewer
    {
        #region Properties

        private readonly Func<Task<bool>> _hasUpdateAsyncCallback;

        private readonly Func<Task<byte[]>> _downloadUpdateAsyncCallback;

        private readonly string _webContentFolder = "wwwroot/pwa";

        private readonly string _temporaryFolder = "wwwroot/tmp";

        #endregion
        
        #region Constructor

        public ContentViewer(Func<Task<bool>> hasUpdateAsyncCallback = null, 
            Func<Task<byte[]>> downloadUpdateAsyncCallback = null)
        {
            _hasUpdateAsyncCallback = hasUpdateAsyncCallback;
            _downloadUpdateAsyncCallback = downloadUpdateAsyncCallback;
        }

        #endregion
        
        #region Methods

        public async Task ShowAsync(WebView webView)
        {
            // Create a folder to store web content.
            var absoluteWebContentDirectory = MakeContentDirectory();
            var temporaryDirectory = MakeTemporaryDirectory();
            
            if (_hasUpdateAsyncCallback != null)
            {
                var hasUpdate = await _hasUpdateAsyncCallback();
                if (hasUpdate && _downloadUpdateAsyncCallback != null)
                {
                    var downloadedContent = await _downloadUpdateAsyncCallback();
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
            using var stream = File.Create(fileName);
            await stream.WriteAsync(content, 0, content.Length);
            await stream.FlushAsync();

            var absoluteFilePath = Path.Combine(designatedFolder, fileName);
            return absoluteFilePath;
        }
        
        #endregion
    }
}