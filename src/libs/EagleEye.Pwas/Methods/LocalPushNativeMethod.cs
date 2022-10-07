using System;
using System.Threading;
using System.Threading.Tasks;
using EagleEye.Pwas.Constants;
using EagleEye.Pwas.Enums;
using EagleEye.Pwas.Interfaces;
using EagleEye.Pwas.Models;
using EagleEye.Pwas.Models.MethodPayloads;
using Newtonsoft.Json;

namespace EagleEye.Pwas.Methods
{
    public class LocalPushNativeMethod : INativeMethod
    {
        #region Properties

        private readonly DesktopNotifications.INotificationManager _notificationManager;
        
        #endregion
        
        #region Constructor

        public LocalPushNativeMethod(DesktopNotifications.INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        #endregion
        
        #region Methods
        
        public virtual async Task<MethodExecutionResult> ExecuteAsync(NativeRequest nativeRequest, 
            string rawValue, CancellationToken cancellationToken = default)
        {
            if (!NativeNamespaces.LocalNotification.Equals(nativeRequest.Namespace))
                return new MethodExecutionResult(ExecutionResults.Skipped);
            
            if (!LocalNotificationMethodNames.Create.Equals(nativeRequest.Method))
                return new MethodExecutionResult(ExecutionResults.Skipped);
            
            var payload = JsonConvert.DeserializeObject<LocalPushNativePayload>(rawValue);
            if (payload == null)
                return new MethodExecutionResult(ExecutionResults.Skipped);

            var systemTime = DateTime.UtcNow;
            var delay = TimeSpan.Zero;
            if (payload.ShowTime > systemTime)
                delay = payload.ShowTime - systemTime;

            var notification = new DesktopNotifications.Notification();
            notification.Body = payload.Body;
            notification.Title = payload.Title;
            await _notificationManager.ShowNotification(notification);
            
            return new MethodExecutionResult(ExecutionResults.Successful);
        }
        
        #endregion
    }
}