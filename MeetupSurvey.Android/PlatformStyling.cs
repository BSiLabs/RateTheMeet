using System;
using Android.App;
using MeetupSurvey.Theming;

namespace MeetupSurvey.Droid
{
    public class PlatformStyling : IPlatformStyling
    {
        public static Activity Activity { get; set; }

        public PlatformStyling()
        {
        }

        public void BarTextDark()
        {

        }

        public void BarTextLight()
        {

        }

        public int GetStatusBarHeight()
        {
            //return 0;
            var statusBarHeight = -1;
            var resourceId = Activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                statusBarHeight = (int)(Activity.Resources.GetDimensionPixelSize(resourceId) / Activity.Resources.DisplayMetrics.Density);
            }
            return statusBarHeight;
        }

        public void ShowStatusBar()
        {

        }
    }
}
