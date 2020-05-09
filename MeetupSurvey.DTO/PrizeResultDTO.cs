using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class PrizeResultDTO
    {
        public string SurveyName { get; set; }
        public DateTimeOffset DatePublished { get; set; }
        public int EntryCount { get; set; }
        public List<WinnerDTO> Entries { get; set; }
        public List<PrizeDTO> Prizes { get; set; }
        public string Image { get; set; }
    }
}
