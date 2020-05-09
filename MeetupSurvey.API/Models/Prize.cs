using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Models
{
    [Table("Prize")]
    public class Prize : EntityBase
    {
        
        public string Name { get; set; }
        public string Details { get; set; }
        public string Photo { get; set; }
        public UserAccount UserAccount { get; set; }
        public string UserAccountId { get; set; }
        public Survey Survey { get; set; }
        public string SurveyId { get; set; }

    }
}
