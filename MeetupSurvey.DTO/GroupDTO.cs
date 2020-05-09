using System;
using System.Collections.Generic;

namespace MeetupSurvey.DTO
{
    public class GroupDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string GroupPhoto { get; set; }
        public string KeyPhoto { get; set; }
        public string NextEventId { get; set; }
        public string EventName { get; set; }
        public string EventTime { get; set; }
        public string EventLink { get; set; }
        public int EventRsvp { get; set; }
        public bool IsAdmin { get; set; }
        public List<MemberDTO> Organizers { get; set; }
        public string UrlName { get; set; }
        public bool GroupSubscribed { get; set; }
    }
}
