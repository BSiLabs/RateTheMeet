using System;
namespace MeetupSurvey.API.ConfigOptions
{
    public class AppSettings
    {
        public CacheDurations CacheDurations { get; set; }
        public string AdminOverrideList { get; set; }
        public bool IsDebug { get; set; }

        public AppSettings()
        {
#if DEBUG
            IsDebug = true;
#endif
        }
    }
    public class CacheDurations
    {
        public int GroupsCache { get; set; }
        public int UsersCache { get; set; }
    }
}
