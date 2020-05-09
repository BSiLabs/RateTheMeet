using System;
namespace MeetupSurvey.Services.PushNotifications
{
    public class Device
    {
        public string Email { get; set; }
        public string Jwt { get; set; }
        public string InstallId { get; set; }
        public bool IsAndroid { get; set; }
    }
}
