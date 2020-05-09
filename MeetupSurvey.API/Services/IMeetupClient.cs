using MeetupSurvey.DTO;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Services
{
    public class MyQueryParams
    {
        public string fields { get; set; }
    }

    public interface IMeetupClient
    {
        [Get("/members/self")]
        Task<MeetupUser> GetUser([Header("Authorization")] string accessToken);

        [Get("/self/groups")]
        Task<List<GroupsResponse>> GetGroups([Header("Authorization")] string accessToken, MyQueryParams myParams);

        [Get("/{urlName}/members?only=id,name,photo,group_profile&page={page}&offset={offset}")]
        Task<List<Member>> GetGroupMembers([Header("Authorization")] string accessToken, string urlName, int page, int offset);

        [Get("/find/upcoming_events?self_groups=only")]
        Task<EventsResponse> GetEvents([Header("Authorization")] string accessToken);

    }

    

}
