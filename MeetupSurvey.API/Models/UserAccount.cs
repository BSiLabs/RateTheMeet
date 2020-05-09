using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Models
{
    //[Table("UserAccount")]
    public class UserAccount : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string MeetupUserId { get; set; }
    }
}
