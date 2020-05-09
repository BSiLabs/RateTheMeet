using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Models
{
    //[Table("GroupUser")]
    public class GroupUser : EntityBase
    {
        public Group Group { get; set; }
        public string GroupId { get; set; }

        public UserAccount UserAccount { get; set; }
        public string UserAccountId { get; set; }
    }
}
