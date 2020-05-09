using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using MeetupSurvey.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API
{
    static public class Helpers
    {
        static public string FormatToken(string token)
        {
            return String.Format("Bearer {0}", token);
        }

        static public string GetFilenameFromUrl(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }

        static public GroupDTO ToDTO(GroupsResponse group)
        {
            if (group == null)
                return null;
            var groupDTO = new GroupDTO()
            {
                Id = group.id,
                Name = group.name,
                Description = group.description,
                Link = group.link,
                GroupPhoto = group.group_photo?.photo_link,
                KeyPhoto = group.key_photo?.photo_link,
                NextEventId = group.next_event?.id,
                //EventRsvp = group.next_event != null ? group.next_event.yes_rsvp_count : 0,
                IsAdmin = (group.self.role == "organizer" || group.self.role == "coorganizer") ? true : false,
                UrlName = group.urlname                
            };

            if(group.organizer != null)
                groupDTO.Organizers = new List<MemberDTO>() { new MemberDTO() { Id = group.organizer.id, Name = group.organizer.name } };
            

            return groupDTO;
        }


        static public MemberDTO ToDTO(Member member)
        {
            return new MemberDTO()
            {
                Name = member.name,
                Photo = member.photo?.photo_link
            };
        }
    }
}
