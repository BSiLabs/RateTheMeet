using System;
namespace MeetupSurvey.Theming
{
    public interface IPlatformStyling
    {
        void BarTextLight();
        void BarTextDark();
        void ShowStatusBar();
        int GetStatusBarHeight();
    }
}
