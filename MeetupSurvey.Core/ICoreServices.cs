using MeetupSurvey.Core.Device;

namespace MeetupSurvey.Core
{
    public interface ICoreServices
    {
        IAppSettings AppSettings { get; }
        IProfile Profile { get; }
        IDialogs Dialogs { get; }
        ILocalize Localize { get; }
        INetwork Network { get; }
        ILogger Log { get; }
        IVersionInfo VersionInfo { get; }
        IWebNavigationService WebNavigationService { get; }
    }
}
