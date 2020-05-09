using System;
using System.Collections.Generic;

namespace MeetupSurvey.Core.Data
{
    public class Survey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserAccountId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
