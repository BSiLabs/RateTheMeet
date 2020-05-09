

using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Autofac;
using MeetupSurvey.Core.Data;

namespace MeetupSurvey.Droid
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "meetupsurvey",
    DataHost = "login")]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Convert Android.Net.Url to Uri
            var uri = new Uri(Intent.Data.ToString());

            // Load redirectUrl page
            var accountService = (App.Container).Resolve<IAccountService>();

            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            accountService.ProcessUri(uri);



            this.Finish();
            return;

        }
    }
}
