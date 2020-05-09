using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class SurveyDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = String.Empty;
        //public bool IsAdmin { get; set; }
        public bool HasCompleted { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public bool Archived { get; set; }
        public double Average { get; set; }
        public int Entries { get; set; }
        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public GroupDTO Group { get; set; }

        public List<PrizeDTO> Prizes { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}
