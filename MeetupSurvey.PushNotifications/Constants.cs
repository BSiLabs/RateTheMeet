using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.PushNotifications
{
    static public class Constants
    {
        static public string BaseUri { get; set; } = "https://api.appcenter.ms";
        static public string ApiKey { get; set; } = "#{AppCenterApiKey}#";
        static public string OrganizationName { get; set; } = "#{AppCenterOrganizationName}#";
    }
}
