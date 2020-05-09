using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class SurveyResultDTO
    {
        public string SurveyName { get; set; }
        public DateTimeOffset DatePublished { get; set; }
        public int Entries { get; set; }
        public List<QuestionResultDTO> QuestionResults { get; set; }
        public string HeaderImage { get; set; }
        public bool Archived { get; set; }
    }
}
