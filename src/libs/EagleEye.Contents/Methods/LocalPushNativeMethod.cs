using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using EagleEye.Contents.Constants;
using EagleEye.Contents.Enums;
using EagleEye.Contents.Interfaces;
using EagleEye.Contents.Models;
using EagleEye.Contents.Models.MethodPayloads;
using Newtonsoft.Json;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace EagleEye.Contents.Methods
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