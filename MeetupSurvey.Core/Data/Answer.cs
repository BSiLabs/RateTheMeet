using System;
namespace MeetupSurvey.Core.Data
{
    public class Answer
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
