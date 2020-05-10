using MeetupSurvey.Core.Device;

namespace MeetupSurvey.Device.Impl
{
    public class VersionInfo : IVersionInfo
    {
        public string CurrentVersion => Xamarin.Essentials.VersionTracking.CurrentVersion;
    }
}