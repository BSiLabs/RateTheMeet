using MeetupSurvey.API.ConfigOptions;
using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using MeetupSurvey.DTO;
using MeetupSurvey.Services.PushNotifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Controllers
{
    public class AuthedControllerBase : ControllerBase
    {
        protected readonly MeetupSurveyContext _context;
        protected readonly IMeetupClient meetupClient;

        private IMemoryCache cache;
        private IOptions<AppSettings> appSettings;

        protected string Token => this.HttpContext.Request.Headers["Authorization"];

        public AuthedControllerBase(IOptions<AppSettings> appSettings, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache memoryCache)
        {
            _context = context;
            this.meetupClient = meetupClient;
            this.cache = memoryCache;
            this.appSettings = appSettings;
        }

        protected async Task<ActionResult<UserAccount>> GetUser()
        {
            var user = GetUserFromCache();
            if (user == null)
            {
                user = await GetUserFromMeetup();
                if (user == null)
                    return Unauthorized();
                else
                {
                    SaveUserToCache(user);
                    return user;
                }
            }
            else
                return user;
        }


        private async Task<UserAccount> GetUserFromMeetup()
        {
            try
            {
                var meetupUser = await meetupClient.GetUser(Token);
                if (meetupUser == null)
                    return null;
                var user = _context.UserAccounts.Where(x => x.MeetupUserId == meetupUser.id).FirstOrDefault();

                _context.Attach(user);

                user.Name = meetupUser.name;
                user.Email = meetupUser.email;
                user.Photo = meetupUser.photo?.photo_link;

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Refit.ApiException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void SaveUserToCache(UserAccount user)
        {
            
            // Look for cache key.
            if (!cache.TryGetValue(Token, out UserAccount existingUser))
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(appSettings.Value.CacheDurations.UsersCache));

                // Save data in cache.
                cache.Set(Token, user, cacheEntryOptions);
            }
        }

        private UserAccount GetUserFromCache()
        {
            this.cache.TryGetValue(Token, out UserAccount user);

            return user;
        }


        protected async Task<List<Device>> GetPushRegistrationsByEmail(string email, MeetupSurveyContext scopedContext = null)
        {
            List<PushRegistration> registrations = null;
            if (scopedContext != null)
                registrations = await scopedContext.PushRegistrations.Where(x => x.Email == email).ToListAsync();
            else
                registrations = await _context.PushRegistrations.Where(x => x.Email == email).ToListAsync();

            return registrations.Select(x => new Device()
            {
                Email = x.Email,
                InstallId = x.InstallId,
                IsAndroid = x.IsAndroid,
                Jwt = x.Jwt
            }).ToList();
        }

        #region Groups Cache and API
        protected async Task<List<GroupDTO>> TryGetGroups(UserAccount user, bool forceUpdate = false)
        {
            List<GroupDTO> returnList;
            List<string> adminList = null;
            if (this.appSettings.Value.AdminOverrideList?.Length > 0)
            {
                adminList = this.appSettings.Value.AdminOverrideList.Split(';').ToList();
            }

            var groups = GetGroupsFromCache();
            if (groups == null || forceUpdate)
            {
                groups = await GetGroupsFromMeetup();
                if (groups == null)
                    return null;
                else
                {
                    var eventResponse = await this.TryGetEvents(user);

                    foreach (var group in groups)
                    {

                        ////This code is to see if the group's organizer is in the app or not, but current the Meetup API isn't returning that value
                        //var organizer = group.Organizers?.FirstOrDefault();
                        //if (organizer != null)
                        //{
                        //    var groupSubscribed = _context.UserAccounts.Where(x => x.MeetupUserId == organizer.Id).FirstOrDefault();
                        //    if (groupSubscribed == null)
                        //        group.GroupSubscribed = true;
                        //    else
                        //        group.GroupSubscribed = false;
                        //}
                        ////////////

                        var mevent = eventResponse.events.Where(x => x.id == group.NextEventId).FirstOrDefault();
                        if (mevent != null)
                        {
                            var convertedDate = Convert.ToDateTime(mevent.local_date);
                            var convertedTime = Convert.ToDateTime(mevent.local_time);

                            var date = new DateTime(convertedDate.Year, convertedDate.Month, convertedDate.Day, convertedTime.TimeOfDay.Hours, convertedTime.TimeOfDay.Minutes, 0);

                            group.EventName = mevent.name;
                            group.EventTime = date.ToString("ddd MMM dd, yyyy") + " @ " + date.ToString("h:mm tt");
                            group.EventLink = mevent.link;
                        }
                    }

                    SaveGroupsToCache(groups);
                    returnList = groups;
                }
            }
            else
                returnList = groups;

            if (adminList != null && adminList.Contains(user.Email))
                foreach (var group in returnList)
                    group.IsAdmin = true;




            



            

            return returnList;
        }

        private async Task<List<GroupDTO>> GetGroupsFromMeetup()
        {
            try
            {
                var meetupGroups = await meetupClient.GetGroups(Token, new MyQueryParams() { fields = "self" });
                if (meetupGroups == null)
                    return null;

                List<GroupDTO> groupList = new List<GroupDTO>();
                foreach (var group in meetupGroups)
                {
                    var dto = Helpers.ToDTO(group);
                    groupList.Add(dto);
                }

                return groupList;
            }
            catch (Refit.ApiException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void SaveGroupsToCache(List<GroupDTO> Groups)
        {
            // Look for cache key.
            if (!cache.TryGetValue("Groups-" + Token, out List<GroupDTO> existingGroups))
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(appSettings.Value.CacheDurations.GroupsCache));

                // Save data in cache.
                cache.Set("Groups-" + Token, Groups, cacheEntryOptions);
            }
        }

        private List<GroupDTO> GetGroupsFromCache()
        {
            this.cache.TryGetValue("Groups-" + Token, out List<GroupDTO> Groups);

            return Groups;
        }

        public async Task<GroupDTO> GetGroup(UserAccount user, string groupId)
        {
            var groups = await TryGetGroups(user);
            var group = groups.Where(x => x.Id == groupId).FirstOrDefault();

            return group;
        }
        #endregion

        #region Events Cache and API
        protected async Task<EventsResponse> TryGetEvents(UserAccount user, bool forceUpdate = false)
        {
            EventsResponse returnList;
            List<string> adminList = null;
            if (this.appSettings.Value.AdminOverrideList?.Length > 0)
            {
                adminList = this.appSettings.Value.AdminOverrideList.Split(';').ToList();
            }

            var events = GetEventsFromCache();
            if (events == null || forceUpdate)
            {
                events = await GetEventsFromMeetup();
                if (events == null)
                    return null;
                else
                {
                    SaveEventsToCache(events);
                    returnList = events;
                }
            }
            else
                returnList = events;


            return returnList;
        }

        private async Task<EventsResponse> GetEventsFromMeetup()
        {
            try
            {
                var meetupEvents = await meetupClient.GetEvents(Token);
                if (meetupEvents == null)
                    return null;

                return meetupEvents;
            }
            catch (Refit.ApiException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void SaveEventsToCache(EventsResponse Events)
        {
            // Look for cache key.
            if (!cache.TryGetValue("Events-" + Token, out EventsResponse existingEvents))
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(appSettings.Value.CacheDurations.GroupsCache));

                // Save data in cache.
                cache.Set("Events-" + Token, Events, cacheEntryOptions);
            }
        }

        private EventsResponse GetEventsFromCache()
        {
            this.cache.TryGetValue("Events-" + Token, out EventsResponse Events);

            return Events;
        }
        #endregion



        protected void ClearUserCache()
        {
            this.cache.Dispose();
        }



        #region ToDTO
        protected SurveyDTO ToDTO(Survey survey, GroupDTO group)
        {
            var surveyDTO = new SurveyDTO()
            {
                Id = survey.Id,
                Name = survey.Name,
                GroupId = survey.GroupId,
                //Commented this out to allow admin toggle to answer own surveys
                //IsAdmin = userId == survey.UserAccountId,
                DatePublished = survey.DatePublished,
                Archived = survey.Archived,
                Questions = survey.Questions.Select(q => new QuestionDTO()
                {
                    Id = q.Id,
                    Name = q.Name,
                    Order = q.Order,
                    SurveyId = q.SurveyId
                }).ToList(),
                Prizes = survey.Prizes.Select(x => new PrizeDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Photo = x.Photo,
                    SurveyId = x.SurveyId
                }).ToList()
                
            };
            if(group != null)
                surveyDTO.Group = group;

            //TODO: Remove This
            //surveyDTO.IsAdmin = true;
            /////////////////////////

            return surveyDTO;
        }

        protected QuestionDTO ToDTO(Question question)
        {
            return new QuestionDTO()
            {
                Id = question.Id,
                Name = question.Name,
                Order = question.Order,
                SurveyId = question.SurveyId
            };
        }

        protected PrizeDTO ToDTO(Prize prize)
        {
            return new PrizeDTO()
            {
                Id = prize.Id,
                Name = prize.Name,
                Photo = prize.Photo,
                SurveyId = prize.SurveyId

            };
        }
        #endregion

        #region ToModel
        protected async Task<Question> ToModel(QuestionDTO questionDTO)
        {
            var question = await _context.Questions.FindAsync(questionDTO.Id);
            if (question == null)
            {
                question = new Question()
                {
                    Id = questionDTO.Id,
                    SurveyId = questionDTO.SurveyId
                };
            }

            question.Name = questionDTO.Name;
            question.Order = questionDTO.Order;

            return question;
        }

        protected async Task<Prize> ToModel(PrizeDTO prizeDTO)
        {
            var prize = await _context.Prizes.FindAsync(prizeDTO.Id);
            if (prize == null)
            {
                prize = new Prize()
                {
                    Id = prizeDTO.Id,
                    SurveyId = prizeDTO.SurveyId,
                    Name = prizeDTO.Name
                };
            }

            prize.Name = prizeDTO.Name;
            prize.SurveyId = prizeDTO.SurveyId;

            ////Only update the photo if it's been removed
            if (prizeDTO.Photo == null)
                prize.Photo = null;

            return prize;
        }

        protected async Task<Survey> ToModel(SurveyDTO surveyDTO)
        {
            var survey = await _context.Surveys.FindAsync(surveyDTO.Id);

            if (survey == null)
            {
                survey = new Survey() { Id = surveyDTO.Id };
            }
            survey.Name = surveyDTO.Name;
            survey.GroupId = surveyDTO.GroupId;
            var modelTask = surveyDTO.Questions.Select(x => ToModel(x));
            survey.Questions = (await Task.WhenAll(modelTask)).ToList();

            var prizeTask = surveyDTO.Prizes.Select(x => ToModel(x));
            survey.Prizes = (await Task.WhenAll(prizeTask)).ToList();
            return survey;
        }
        #endregion
    }
}
