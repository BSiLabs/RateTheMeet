using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using MeetupSurvey.Core.Device;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Native = Microsoft.AppCenter.AppCenter;

namespace MeetupSurvey.Droid.Services
{
    public class NotificationsImpl : INotifications
    {
        const string CHANNEL_ID = "rtm";

        readonly Subject<MeetupSurvey.DTO.Notification> prizeNotificationSubject;
        readonly Subject<MeetupSurvey.DTO.Notification> newSurveyNotificationSubject;

        public NotificationsImpl()
        {
            this.prizeNotificationSubject = new Subject<MeetupSurvey.DTO.Notification>();
            this.newSurveyNotificationSubject = new Subject<MeetupSurvey.DTO.Notification>();
        }
        public IObservable<MeetupSurvey.DTO.Notification> WhenPrizeNotified() => this.prizeNotificationSubject;
        public IObservable<MeetupSurvey.DTO.Notification> WhenNewSurvey() => this.newSurveyNotificationSubject;

        public async Task<string> GetPushId()
        {
            var installId = await Native.GetInstallIdAsync();
            return installId?.ToString();
        }


        public Task<bool> RequestPermission()
        {
            Native.Start(typeof(Push));
            return Task.FromResult(NotificationManagerCompat.From(Application.Context).AreNotificationsEnabled());
        }


        public Task CancelAll()
        {
            NotificationManagerCompat
                .From(Application.Context)
                .CancelAll();

            return Task.CompletedTask;
        }


        public Task Send(string title, string message, IDictionary<string,string> customData)
        {
            var notificationId = 1;
            var context = Application.Context;

            var launchIntent = context
                .PackageManager
                .GetLaunchIntentForPackage(context.PackageName);

            var pendingIntent = PendingIntent.GetActivity(
                context,
                notificationId,
                launchIntent,
                PendingIntentFlags.CancelCurrent
            );

            var builder = new NotificationCompat.Builder(context)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetAutoCancel(true)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Mipmap.ic_notification)
                //.SetColor(Resource.Color.notification_col)
                .SetContentIntent(pendingIntent);
            
            if ((int)Build.VERSION.SdkInt >= 26)
            {
                var nativeManager = NotificationManager.FromContext(context);
                var channel = nativeManager.GetNotificationChannel(CHANNEL_ID);
                if (channel == null)
                {
                    channel = new NotificationChannel(
                        CHANNEL_ID,
                        "RateTheMeet",
                        NotificationImportance.Default
                    );
                    nativeManager.CreateNotificationChannel(channel);
                }
                builder.SetChannelId(CHANNEL_ID);
            }
            
            var nativeNotification = builder.Build();
            NotificationManagerCompat
                .From(context)
                .Notify(notificationId, nativeNotification);

            if (customData != null && customData.ContainsKey("WinnerId"))
                prizeNotificationSubject.OnNext(new MeetupSurvey.DTO.Notification() { Title = title, Message = message, CustomData = customData });
            else
                newSurveyNotificationSubject.OnNext(new MeetupSurvey.DTO.Notification() { Title = title, Message = message, CustomData = customData });

            return Task.CompletedTask;
        }


        public void OpenSettings()
        {
            var c = Application.Context;

            var intent = new Intent();
            intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddCategory(Intent.CategoryDefault);
            intent.SetData(Android.Net.Uri.Parse("package:" + c.PackageName));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.NoHistory);
            intent.AddFlags(ActivityFlags.ExcludeFromRecents);
            c.StartActivity(intent);
        }


        public async Task<NotificationState> GetStatus()
        {
            var result = await this.RequestPermission();
            var r = result ? NotificationState.Granted : NotificationState.Denied;

            return r;
        }
    }
}
