using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class QuestionDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string SurveyId { get; set; }
        public bool Deleted { get; set; }
    }
}
