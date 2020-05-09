using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupSurvey.Services.PushNotifications
{
    public class AppCenterNotification
    {
        [JsonProperty("notification_content")]
        public NotificationContent Content { get; set; } = new NotificationContent();

        [JsonProperty("notification_target")]
        public NotificationTarget Target { get; set; } = new NotificationTarget(); // null is all
    }


    public class NotificationContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public Dictionary<string,string> CustomData { get; set; }
    }


    public class NotificationTarget
    {
        public const string Device = "devices_target";

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("devices")]
        public string[] Devices { get; set; }
    }
}

