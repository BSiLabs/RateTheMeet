using MeetupSurvey.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Services
{
    public class AuthTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class GroupsResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string urlname { get; set; }
        public string link { get; set; }
        public Photo group_photo { get; set; }
        public Photo key_photo { get; set; }
        public Event next_event { get; set; }
        public Member organizer { get; set; }
        public Self self { get; set; }
    }

    public class EventsResponse
    {
        public List<events> events { get; set; }
    }

    public class events
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string local_date { get; set; }
        public string local_time { get; set; }
        //public Venue venue { get; set; }
        public string link { get; set; }
        public string visibility { get; set; }
    }

    public class Venue
    {
        public string address_1 { get; set; }
        public string city { get; set; }
    }

    public class GroupProfile
    {
        public string role { get; set; }
    }


    public class Member
    {
        public string id { get; set; }
        public string name { get; set; }
        public Photo photo { get; set; }
        public GroupProfile group_profile { get; set; }
    }

    public class Self
    {
        public string role { get; set; }
    }

    public class Photo
    {
        public string photo_link { get; set; }
    }

    public class Event
    {
        public string id { get; set; }
        public string name { get; set; }
        public string time { get; set; }
        public int yes_rsvp_count { get; set; }
    }

    public class MeetupAccessArgs
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string code { get; set; }
    }

    public class MeetupRefreshArgs
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string refresh_token { get; set; }
    }

    public class MeetupUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public Photo photo { get; set; }
    }

}
