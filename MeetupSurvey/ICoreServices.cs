using System;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Device;

namespace MeetupSurvey
{
    public interface ICoreServices
    {
        IAppSettings AppSettings { get; }
        IProfile Profile { get; }
        IDialogs Dialogs { get; }
        ILocalize Localize { get; }
        INetwork Network { get; }
        ILogger Log { get; }
    }
}
