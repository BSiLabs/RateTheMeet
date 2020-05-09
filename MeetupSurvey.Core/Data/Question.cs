using System;
namespace MeetupSurvey.Core.Data
{
    public class Question
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SurveyId { get; set; }
        public int Order { get; set; }
    }
}
