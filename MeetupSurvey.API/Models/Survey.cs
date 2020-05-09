using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupSurvey.API.Models
{
    //[Table("Survey")]
    public class Survey : EntityBase
    {
        public string Name { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public bool Archived { get; set; }

        //public Group Group { get; set; }
        public string GroupId { get; set; }


        public UserAccount UserAccount { get; set; }
        public string UserAccountId { get; set; }

        public List<Question> Questions { get; set; }
        public List<Prize> Prizes { get; set; }
    }
}
