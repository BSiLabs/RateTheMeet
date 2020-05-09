using System;
namespace MeetupSurvey.DTO
{
    public class PushRegisterArgs
    {
        public bool IsUnRegister { get; set; }
        public string Email { get; set; }
        public string Jwt { get; set; }
        public string InstallId { get; set; }
        public bool IsAndroid { get; set; }
    }
}
