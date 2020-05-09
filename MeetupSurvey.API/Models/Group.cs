using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Models
{
    //[Table("Group")]
    public class Group : EntityBase
    {
        public string Name { get; set; }
        public string MeetupGroupId { get; set; }
    }
}
