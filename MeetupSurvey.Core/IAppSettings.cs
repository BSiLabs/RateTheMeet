using System;
using System.ComponentModel;

namespace MeetupSurvey.Core
{
    public interface IAppSettings : INotifyPropertyChanged
    {
        string BaseApiUri { get; set; }

        string MeetupAuthUri { get; set; }
        string MeetupBaseUri { get; set; }

        string MeetupClientId { get; set; }
        string MeetupRedirectUri { get; set; }
        string MeetupGrantType { get; set; }
        string MeetupRefreshGrantType { get; set; }
        string MeetupTokenUri { get; set; }
        //Settings
        bool IsBiometricsEnabled { get; set; }

        //Permissions
        bool HasAcceptedDataPrivacy { get; set; }
        bool HasViewedIntro { get; set; }
    }
}
