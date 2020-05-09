using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Autofac;
using Java.Interop;
using MeetupSurvey.Core.Device;
using MeetupSurvey.Droid.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;

namespace MeetupSurvey.Droid
{
    [Activity(
    Label = "RateTheMeet",
    Icon = "@mipmap/ic_launcher",
    RoundIcon = "@mipmap/ic_round_launcher",
    Theme = "@style/MyTheme.Splash",
    MainLauncher = true
)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Push.PushNotificationReceived += async (sender, e) =>
            {
                var notifications = App.Container.Resolve<INotifications>();
                await notifications.Send(e.Title, e.Message, e.CustomData);
            };
            AppCenter.Start(MeetupSurvey.Logging.AppCenter.Constants.AppCenterToken, typeof(Push));

            base.OnCreate(savedInstanceState);
            var intent = new Intent(Application.Context, typeof(MainActivity));
            this.StartActivity(intent);
            this.Finish();
        }
    }
}
