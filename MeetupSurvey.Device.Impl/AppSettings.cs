using System;
using MeetupSurvey.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Device.Impl
{
    public class AppSettings : ReactiveObject, IAppSettings
    {
        [Reactive] public string BaseApiUri { get; set; } = Constants.BaseApiUri;

        [Reactive] public string MeetupAuthUri { get; set; } = Constants.MeetupAuthUri;
        [Reactive] public string MeetupBaseUri { get; set; } = Constants.MeetupBaseUri;

        [Reactive] public string MeetupTokenUri { get; set; } = Constants.MeetupTokenUri;
        [Reactive] public string MeetupRedirectUri { get; set; } = Constants.MeetupRedirectUri;
        [Reactive] public string MeetupClientId { get; set; } = Constants.MeetupClientId;
        [Reactive] public string MeetupGrantType { get; set; } = Constants.MeetupGrantType;
        [Reactive] public string MeetupRefreshGrantType { get; set; } = Constants.MeetupRefreshGrantType;

        [Reactive] public bool IsBiometricsEnabled { get; set; }
        [Reactive] public bool HasAcceptedDataPrivacy { get; set; }
        [Reactive] public bool HasViewedIntro { get; set; }
    }
}
