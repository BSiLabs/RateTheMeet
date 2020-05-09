using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class AuthUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string MeetupUserId { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public DateTimeOffset Expiry { get; set; }
    }
}
