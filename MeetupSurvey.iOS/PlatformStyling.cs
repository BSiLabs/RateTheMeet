using System;
using MeetupSurvey.Theming;
using UIKit;

namespace MeetupSurvey.iOS
{
    public class IOSPlatformStyling : IPlatformStyling
    {
        public void BarTextDark()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
        }

        public void BarTextLight()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
        }

        public int GetStatusBarHeight()
        {
            var height = (int)UIApplication.SharedApplication.StatusBarFrame.Height;
            //Don't think we need the height for our purposes, this is mainly for Android, kept the code here incase we do
            return height;
        }

        public void ShowStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }
    }
}
