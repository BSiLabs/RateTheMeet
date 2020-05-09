using System;
namespace MeetupSurvey.Logging.AppCenter
{
    public static class Constants
    {
#if DEBUG
        // RateTheMeet - Debug
        public const string AppCenterToken =
            "#{AppCenterDebugToken}#";
#else
        public const string AppCenterToken = "#{AppCenterToken}#";
#endif
    }
}
