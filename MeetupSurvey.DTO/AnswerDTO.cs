using System;
namespace MeetupSurvey.DTO
{
    public class AnswerDTO
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
