using System;
using System.IO;
using EagleEye.Contents.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace EagleEye.Contents.Services
{
    public class Startup : IStartup
    {
        #region Methods
        
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var webContentDirectory = Path.Combine(currentDirectory, Folders.WebContent);

            var defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            defaultFilesOptions.FileProvider = new PhysicalFileProvider(webContentDirectory);
            app.UseDefaultFiles(defaultFilesOptions);
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(webContentDirectory)
            });
        }
        
        #endregion
    }
}