using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Models
{
    public class PushRegistration : EntityBase
    {
        public string Email { get; set; }
        public string Jwt { get; set; }
        public string InstallId { get; set; }
        public bool IsAndroid { get; set; }
    }
}
