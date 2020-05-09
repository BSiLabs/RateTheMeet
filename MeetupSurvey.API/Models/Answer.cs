using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupSurvey.API.Models
{
    //[Table("Answer")]
    public class Answer : EntityBase
    {
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string QuestionId { get; set; }
        public Question Question { get; set; }

        public string UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}
