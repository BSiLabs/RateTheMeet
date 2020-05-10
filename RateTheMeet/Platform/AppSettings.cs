using MeetupSurvey.Core;
using ReactiveUI;

namespace RateTheMeet.Platform
{
    public class AppSettings : ReactiveObject, IAppSettings
    {
        public string BaseApiUri { get; set; } = Constants.BaseApiUri;

        public string MeetupAuthUri { get; set; } = Constants.MeetupAuthUri;
        public string MeetupBaseUri { get; set; } = Constants.MeetupBaseUri;

        public string MeetupTokenUri { get; set; } = Constants.MeetupTokenUri;
        public string MeetupRedirectUri { get; set; } = Constants.MeetupRedirectUri;
        public string MeetupClientId { get; set; } = Constants.MeetupClientId;
        public string MeetupGrantType { get; set; } = Constants.MeetupGrantType;
        public string MeetupRefreshGrantType { get; set; } = Constants.MeetupRefreshGrantType;

        public bool IsBiometricsEnabled { get; set; }
        public bool HasAcceptedDataPrivacy { get; set; }
        public bool HasViewedIntro { get; set; }
    }
}
