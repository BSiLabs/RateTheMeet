using System.Collections.Generic;

namespace MeetupSurvey.DTO
{
    public class QuestionResultDTO
    {
        public string Name { get; set; }
        public List<int> Ratings { get; set; }
        public double AverageRating { get; set; }
        public List<string> Comments { get; set; }
    }
}