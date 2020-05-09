using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.DTO
{
    public class PrizeDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string SurveyId { get; set; }
        public string WinnerName { get; set; }
        public string WinnerUserAccountId { get; set; }
        public string WinnerPhoto { get; set; }
        public bool Deleted { get; set; }
    }
}
