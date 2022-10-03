using System;
using System.Linq;
using System.Threading.Tasks;
using EagleEye.Contents.Enums;
using EagleEye.Contents.Interfaces;
using EagleEye.Contents.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebViewControl;

namespace EagleEye.Contents.Handlers
{
    internal class MethodExecutor
    {
        #region Properties

        private readonly IServiceProvider _serviceProvider;
        
        #endregion
        
        #region Constructor

        public MethodExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        #endregion
        
        #region Methods

        public virtual void PostMessage(string szNativeRequest)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            
            // Get the native methods
            var nativeMethods = serviceProvider.GetServices<INativeMethod>().ToArray();
            if (!nativeMethods.Any())
                return;

            var webView = serviceProvider.GetService<WebView>()!;
            var nativeRequest = JsonConvert.DeserializeObject<NativeRequest<object>>(szNativeRequest)!;
            foreach (var nativeMethod in nativeMethods)
            {
                var executionResult = nativeMethod.ExecuteAsync(nativeRequest, JsonConvert.SerializeObject(nativeRequest.Data)).Result;
                if (executionResult.Result == ExecutionResults.Skipped)
                    continue;

                var nativeResponse = new NativeResponse<object>(nativeRequest.Id);
                nativeResponse.Namespace = $"mobile-response_{nativeRequest.Namespace}";
                nativeResponse.Method = nativeRequest.Method;
                nativeResponse.Data = executionResult.Payload;

                var szMessage = JsonConvert.SerializeObject(nativeResponse);

                webView.ExecuteScript($"window.postMessage({szMessage})");
                
                return;
            }
        }

        #endregion
    }
}