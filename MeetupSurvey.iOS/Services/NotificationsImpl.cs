using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Foundation;
using MeetupSurvey.Core.Device;
using MeetupSurvey.DTO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using UIKit;
using UserNotifications;

namespace MeetupSurvey.iOS.Services
{
    public class NotificationsImpl : INotifications
    {
        readonly Subject<MeetupSurvey.DTO.Notification> prizeNotificationSubject;
        readonly Subject<MeetupSurvey.DTO.Notification> newSurveySubject;

        public IObservable<MeetupSurvey.DTO.Notification> WhenPrizeNotified() => this.prizeNotificationSubject;
        public IObservable<MeetupSurvey.DTO.Notification> WhenNewSurvey() => this.newSurveySubject;

        public NotificationsImpl()
        {
            this.prizeNotificationSubject = new Subject<Notification>();
            this.newSurveySubject = new Subject<Notification>();
        }

        public async Task<string> GetPushId()
        {
            var installId = await AppCenter.GetInstallIdAsync();
            return installId?.ToString();
        }


        public Task CancelAll() => this.Invoke(() =>
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        });


        public async Task<NotificationState> GetStatus()
        {
            var status = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            switch (status.AuthorizationStatus)
            {
                case UNAuthorizationStatus.NotDetermined:
                    return NotificationState.NotDetermined;

                case UNAuthorizationStatus.Denied:
                    return NotificationState.Denied;

                //case UNAuthorizationStatus.Provisional:
                case UNAuthorizationStatus.Authorized:
                default:
                    return NotificationState.Granted;
            }
        }


        public void OpenSettings() => UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));


        public async Task<bool> RequestPermission()
        {
            AppCenter.Start(typeof(Push));
            var tcs = new TaskCompletionSource<bool>();

            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Sound,
                (approved, error) =>
                {
                    if (error != null)
                        tcs.SetException(new Exception(error.Description));
                    else
                        tcs.SetResult(approved);
                });

            var installId = await AppCenter.GetInstallIdAsync();
            if (installId != null)
            {
                // TODO: register
            }

            return await tcs.Task;
        }


        public async Task Send(string title, string message, IDictionary<string, string> customData)
        {
            //var content = new UNMutableNotificationContent
            //{
            //    Title = title,
            //    Body = message,
            //    Badge = 1,
            //    LaunchImageName = "",
            //    Subtitle = ""
            //};
            //var request = UNNotificationRequest.FromIdentifier(
            //    Guid.NewGuid().ToString(),
            //    content,
            //    UNTimeIntervalNotificationTrigger.CreateTrigger(1, false)
            //);
            //await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);

            if(customData != null && customData.ContainsKey("WinnerId"))
                prizeNotificationSubject.OnNext(new MeetupSurvey.DTO.Notification() { Title = title, Message = message, CustomData = customData });
        }


        async Task Invoke(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            if (NSThread.Current.IsMainThread)
                this.Execute(tcs, action);
            else
                NSRunLoop.Main.BeginInvokeOnMainThread(() => this.Execute(tcs, action));

            await tcs.Task.ConfigureAwait(false);
        }


        void Execute(TaskCompletionSource<object> tcs, Action action)
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }

    }
}
