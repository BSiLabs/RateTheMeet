using System;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(MeetupSurvey.iOS.AppVersionProvider))]
namespace MeetupSurvey.iOS
{
   
        public class AppVersionProvider : IAppVersionProvider
        {
            public string AppVersion => NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();
        }

}
