using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupSurvey.API.Models
{
    //[Table("Question")]
    public class Question : EntityBase
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }
        
    }
}
