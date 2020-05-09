using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MeetupSurvey.DTO;
using ReactiveUI;

namespace MeetupSurvey.Survey
{
    public class GroupVM : ReactiveObject
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
        public int EventRsvp { get; set; }
        public bool IsAdmin { get; set; }


        public GroupVM(GroupDTO group)
        {
            this.Name = group.Name;
            this.Id = group.Id;
            this.Description = group.Description;
            this.Link = group.Link;
            this.GroupPhoto = group.GroupPhoto;
            this.KeyPhoto = group.KeyPhoto;
            this.NextEventId = group.NextEventId;
            this.EventName = group.EventName;
            this.EventTime = group.EventTime;
            this.EventRsvp = group.EventRsvp;
            this.IsAdmin = group.IsAdmin;
        }
    }
}
