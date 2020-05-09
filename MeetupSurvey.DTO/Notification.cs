using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class Notification
    {
        public string Email { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public IDictionary<string, string> CustomData {get;set;}
    }
}
