using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(MeetupSurvey.Droid.AppVersionProvider))]
namespace MeetupSurvey.Droid
{


        public class AppVersionProvider : IAppVersionProvider
        {
            public string AppVersion
            {
                get
                {
                    var context = Android.App.Application.Context;
                    var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                    return $"{info.VersionName}.{info.VersionCode.ToString()}";
                }
            }
        }

}
