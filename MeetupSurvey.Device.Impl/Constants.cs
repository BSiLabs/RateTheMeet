using System;
namespace MeetupSurvey.Device.Impl
{
    public static class Constants
    {
#if DEBUG
        public const string BaseApiUri = "http://192.168.0.14:5000/api";
#else
        public const string BaseApiUri = "#{BaseApiUri}#";
#endif

        public const string MeetupBaseUri = "https://api.meetup.com/";
        public const string MeetupAuthUri = "https://secure.meetup.com/oauth2/authorize";
        public const string MeetupTokenUri = "https://secure.meetup.com/oauth2/access";
        public const string MeetupRedirectUri = "#{MeetupRedirectUri}#";
        public const string MeetupClientId = "#{MeetupClientId}#";
        public const string MeetupGrantType = "authorization_code";
        public const string MeetupRefreshGrantType = "refresh_token";
    }
}
