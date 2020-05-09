using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class AuthToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public DateTimeOffset expiry { get; set; }
    }
}
