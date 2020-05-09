using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using MeetupSurvey.DTO;
using MeetupSurvey.API.Services;
using Microsoft.Extensions.Caching.Memory;
using MeetupSurvey.Services.PushNotifications;
using Microsoft.Extensions.Options;
using MeetupSurvey.API.ConfigOptions;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : AuthedControllerBase
    {
        protected readonly IPushNotifications push;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IServiceProvider serviceProvider;

        public SurveysController(IOptions<AppSettings> options, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache, IPushNotifications push, IServiceProvider serviceProvider) : base(options, context, meetupClient, cache)
        {
            this.push = push;
            this.appSettings = options;
            this.serviceProvider = serviceProvider;
        }

        private async Task<List<SurveyDTO>> RetrieveSurveys(UserAccount user, bool archived)
        {
            var groupList = await TryGetGroups(user);
            if (groupList == null)
                return null;
             
            //var groupList = await GetGroupList(user);

            var groupIds = groupList.Select(x => x.Id);

            var dtoList = (await _context.Surveys
                .Where(x => groupIds.Contains(x.GroupId) && x.Archived == archived)
                .Include(x => x.Questions)
                .Include(x => x.Prizes)
                .ToListAsync())
                .Select(x => ToDTO(x, groupList.Where(y => y.Id == x.GroupId).FirstOrDefault()))
                .Where(survey => (!survey.Group.IsAdmin && survey.DatePublished != null && !survey.Archived) || survey.Group.IsAdmin || appSettings.Value.IsDebug) // if not an admin, the survey must be published and not archived
                .ToList();

            List<SurveyDTO> returnList = new List<SurveyDTO>();
            foreach (var survey in dtoList)
            {
                // get the answers associated to the current survey
                var questionIds = survey.Questions.Select(x => x.Id);
                var surveyAnswers = _context.Answers.Where(x => questionIds.Contains(x.QuestionId)).ToList();

                //No need to ensure IsAdmin for stats, this causes a slight issue in debug, we'll keep it here incase we decide otherwise
                //if (survey.Group.IsAdmin)
                    CalculateSurveyStats(surveyAnswers, survey);
                
                
                if (surveyAnswers.Any(x => x.UserAccountId == user.Id))
                    survey.HasCompleted = true;

                survey.Group = groupList.FirstOrDefault(x => x.Id == survey.GroupId);
                returnList.Add(survey);
            }

            return returnList;
        }

        private static void CalculateSurveyStats(List<Answer> surveyAnswers, SurveyDTO survey)
        {
            if (surveyAnswers.Any())
                survey.Average = Math.Round(surveyAnswers.Average(x => x.Rating), 1);
            else
                survey.Average = 0;
            survey.Entries = surveyAnswers.Select(x => x.UserAccountId).Distinct().Count();
        }

        [HttpGet("archived")]
        public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetArchivedSurveys()
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            return await RetrieveSurveys(user.Value, true);
        }

        // GET: api/Survey
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetSurveys()
        {
            var user = await this.GetUser();
            if(user.Value == null)
                return user.Result;

            return await RetrieveSurveys(user.Value, false);
        }

        // GET: api/Survey/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyDTO>> GetSurvey(string id)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await GetSurveyIncludeAll(id);

            if (survey == null)
                return NotFound();
            

            var groups = await this.TryGetGroups(user.Value);
            var group = groups.Where(x => x.Id == survey.GroupId).FirstOrDefault();

            if(group == null) //If group wasn't found, might need to refresh cache
            {
                groups = await this.TryGetGroups(user.Value, true);
                group = groups.Where(x => x.Id == survey.GroupId).FirstOrDefault();
            }

            var surveyDTO = ToDTO(survey, group);
            if (group != null)
                surveyDTO.GroupName = group.Name;

            return surveyDTO;
        }

        // PUT: api/Survey/5
        [HttpPut]
        public async Task<ActionResult<SurveyDTO>> PutSurvey([FromBody] SurveyDTO surveyDTO)
        {
            if (surveyDTO == null)
            {
                return BadRequest();
            }

            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            //Remove the deleted questions
            foreach (var question in surveyDTO.Questions)
            {
                if (question.Deleted)
                {
                    var model = await ToModel(question);
                    _context.Entry(model).State = EntityState.Deleted;
                }
            }
            surveyDTO.Questions.RemoveAll(x => x.Deleted);
            //////
            ///

            List<string> deleteBlobList = new List<string>();

            //Remove the deleted prizes
            foreach (var prize in surveyDTO.Prizes)
            {
                
                if (prize.Deleted)
                {
                    var model = await ToModel(prize);
                    if(!String.IsNullOrEmpty(prize.Photo))
                        deleteBlobList.Add(Helpers.GetFilenameFromUrl(prize.Photo));
                    _context.Entry(model).State = EntityState.Deleted;
                    continue;
                }

                if (String.IsNullOrEmpty(prize.Photo))
                {
                    var existingPrize = await _context.Prizes.FindAsync(prize.Id);
                    if(existingPrize != null && !String.IsNullOrEmpty(existingPrize.Photo))
                        deleteBlobList.Add(Helpers.GetFilenameFromUrl(existingPrize.Photo));
                }
            }
            surveyDTO.Prizes.RemoveAll(x => x.Deleted);
            //////

            var survey = await ToModel(surveyDTO);

            _context.Entry(survey).State = EntityState.Modified;



            

            try
            {
                await _context.SaveChangesAsync();

                await BlobService.DeleteBlobs(user.Value.Id, deleteBlobList);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(survey.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var refreshedModel = await GetSurveyIncludeAll(survey.Id);
            
            return Ok(ToDTO(refreshedModel, surveyDTO.Group));
        }

        // POST: api/Survey
        [HttpPost]
        public async Task<ActionResult<SurveyDTO>> PostSurvey([FromBody] SurveyDTO surveyDTO)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await ToModel(surveyDTO);
            if(survey.UserAccountId == null)
                survey.UserAccountId = user.Value.Id;

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            var group = await GetGroup(user.Value, survey.GroupId);
            return Ok(ToDTO(survey, group));


            //return CreatedAtAction("GetSurvey", new { id = survey.Id }, surveyDTO);
        }

        // DELETE: api/Survey/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSurvey(string id)
        {
            try
            {
                var user = await this.GetUser();
                if (user.Value == null)
                    return user.Result;

                var survey = await GetSurveyIncludeAll(id);
                if (survey == null)
                {
                    return NotFound();
                }

                _context.Surveys.Remove(survey);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }



        private bool SurveyExists(string id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }

        private async Task<Survey> GetSurveyIncludeAll(string id)
        {
            var survey = (await _context.Surveys
                .Where(x => x.Id == id)
                .Include(x => x.Questions)
                .Include(x => x.Prizes)
                .ToListAsync())
                .FirstOrDefault();

            return survey;
        }

        [HttpGet("completed")]
        public async Task<ActionResult<string>> GetCompletedSurveys()
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;
            
            var uniqueSurveys = _context.Answers
                            .Where(x => x.UserAccountId == user.Value.Id)
                            .Join(_context.Questions, x => x.QuestionId, y => y.Id, (x, y) => new { Answer = x, Question = y })
                            .Select(x => x.Question.SurveyId)
                            .Distinct();

           return uniqueSurveys.Count().ToString();
        }

        [HttpGet("result/{id}")]
        public async Task<ActionResult<SurveyResultDTO>> GetSurveyResult(string id)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await _context.Surveys.FindAsync(id);

            var groupList = await TryGetGroups(user.Value);
            var groupIds = groupList.Select(x => x.Id);

            var group = groupList.Where(x => x.Id == survey.GroupId).FirstOrDefault();

            //TODO: Enforce admin requirement after testing
            if (group == null)// || group.IsAdmin == false)
                return BadRequest();

            


            var questionResults = _context.Answers
                .Join(_context.Questions, x => x.QuestionId, y => y.Id, (x, y) => new { Answer = x, Question = y })
                .Where(x => x.Question.SurveyId == survey.Id)
                .GroupBy(x => x.Question.Name)
                .Select(x => new QuestionResultDTO()
                {
                    Name = x.Key,
                    AverageRating = x.Where(z => z.Answer != null).Average(y => y.Answer.Rating),
                     Ratings = x.Where(z => z.Answer != null).Select(y => y.Answer.Rating).ToList(),
                      Comments = x.Where(z => z.Answer != null).Select(y => y.Answer.Comment).ToList()
                });

            return new SurveyResultDTO()
            {
                SurveyName = survey.Name,
                DatePublished = (DateTimeOffset)survey.DatePublished,
                HeaderImage = group.KeyPhoto,
                QuestionResults = questionResults.ToList(),
                Entries = questionResults.FirstOrDefault() == null ? 0 : questionResults.FirstOrDefault().Ratings.Count,
                Archived = survey.Archived
            };
            
        }

        [HttpGet("publish/{id}")]
        public async Task<ActionResult> PublishSurvey(string id)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await _context.Surveys.FindAsync(id);
            _context.Attach(survey);

            survey.DatePublished = DateTimeOffset.Now;

            await _context.SaveChangesAsync();


            var groups = await this.TryGetGroups(user.Value);
            var group = groups.Where(x => x.Id == survey.GroupId).FirstOrDefault();
            SendPublishNotifications(survey, group);

            return Ok();
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishSurvey([FromBody] PublishSurveyArgs publishArgs)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await _context.Surveys.FindAsync(publishArgs.SurveyId);
            _context.Attach(survey);

            if (publishArgs.Unpublish) //If unpublish, delete all answers
            {
                survey.DatePublished = null;
                var questionsIds = await _context.Questions.Where(x => x.SurveyId == publishArgs.SurveyId).Select(x => x.Id).ToListAsync();
                var answers = await _context.Answers.Where(x => questionsIds.Contains(x.QuestionId)).ToListAsync();
                _context.Answers.RemoveRange(answers);
            }
            else
                survey.DatePublished = DateTimeOffset.Now;

            await _context.SaveChangesAsync();

            if (publishArgs.Notify)
            {
                var groups = await this.TryGetGroups(user.Value);
                var group = groups.Where(x => x.Id == survey.GroupId).FirstOrDefault();
                SendPublishNotifications(survey, group);
            }

            return Ok();
        }

        private async void SendPublishNotifications(Survey survey, GroupDTO group)
        {
            using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>()
                        .CreateScope())
            {
                var taskContext = serviceScope.ServiceProvider.GetService<MeetupSurveyContext>();
                var taskMeetupClient = serviceScope.ServiceProvider.GetService<IMeetupClient>();

                var allAppUsers = await taskContext.UserAccounts.ToListAsync();

                int page = 1000;
                int offset = 0;
                bool moreItems = true;

                List<Member> members = new List<Member>();
                while(moreItems)
                {
                    var memberResults = await taskMeetupClient.GetGroupMembers(this.Token, group.UrlName, page, offset);
                    if (memberResults.Count < page)
                        moreItems = false;
                    else
                        offset++;

                    members.AddRange(memberResults);
                }
                

                var idList = members.Select(y => y.id).ToList();

                var usersInApp = allAppUsers.Where(x => idList.Contains(x.MeetupUserId)).ToList();

                var customData = new Dictionary<string, string>()
                {
                    { "NotificationType", nameof(NotificationType.NewSurvey) },
                    { "SurveyId", survey.Id }
                };

                List<Task> taskList = new List<Task>();
                foreach (var userInApp in usersInApp)
                {
                    //Remove this after
                    //if(this.appSettings.Value.AdminOverrideList.Contains(userInApp.Email))
                    //if (userInApp.Email == "matt.fara@hotmail.com")
                    //{
                        var devices = await this.GetPushRegistrationsByEmail(userInApp.Email, taskContext);
                        taskList.Add(push.Send(devices, group.Name, "A new survey was published!", customData));
                    //}
                }
                Task.WhenAll(taskList);
            }
        }

        [HttpGet("archive/{id}")]
        public async Task<ActionResult> ArchiveSurvey(string id)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await _context.Surveys.FindAsync(id);
            _context.Attach(survey);

            survey.Archived = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("prize/startdraw/{prizeId}")]
        public async Task<ActionResult<WinnerDTO>> StartPrizeDraw(string prizeId)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var prize = await _context.Prizes.FindAsync(prizeId);
            var winnerIds = await _context.Prizes.Where(x => x.SurveyId == prize.SurveyId && x.UserAccountId != null).Select(x => x.UserAccountId).ToListAsync();
            
            var survey = await _context.Surveys.FindAsync(prize.SurveyId);

            var groupList = await TryGetGroups(user.Value);
            var groupIds = groupList.Select(x => x.Id);

            var group = groupList.Where(x => x.Id == survey.GroupId).FirstOrDefault();

            //TODO: Force admin
            if (group == null)// || group.IsAdmin == false)
                return BadRequest();

            var surveyAnswers = from a in _context.Answers
                                join q in _context.Questions on a.QuestionId equals q.Id
                                join u in _context.UserAccounts on a.UserAccountId equals u.Id
                                where q.SurveyId == survey.Id
                                select new { Question = q, UserAccount = u, Answer = a };

            var entries = surveyAnswers.GroupBy(x => x.UserAccount.Id)
            .Select(x => new WinnerDTO()
            {
                UserAccountId = x.Key,
                Name = x.FirstOrDefault().UserAccount.Name,
                Photo = x.FirstOrDefault().UserAccount.Photo,
                Email = x.FirstOrDefault().UserAccount.Email
            }).ToList();

            entries = entries.Where(x => !winnerIds.Contains(x.UserAccountId)).ToList();

            var random = new Random();
            int index = random.Next(entries.Count);

            var winner = entries[index];

            _context.Attach(prize);
            prize.UserAccountId = winner.UserAccountId;
            await _context.SaveChangesAsync();





            ////TODO: Send notification
            var winnerUserAccount = await _context.UserAccounts.FindAsync(winner.UserAccountId);


            var customData = new Dictionary<string, string>()
            {
                {"NotificationType", nameof(NotificationType.PrizeDraw) },
                {"PrizeId", prize.Id },
                {"WinnerId", winnerUserAccount.Id },
                {"WinnerName", winnerUserAccount.Name },
                {"WinnerPhoto", winnerUserAccount.Name },
                {"SurveyId", prize.SurveyId }
            };

            List<Task> notificationTaskList = new List<Task>();
            foreach(var entry in entries)
            {
                var devices = await this.GetPushRegistrationsByEmail(entry.Email);
                notificationTaskList.Add(push.Send(devices, "A Prize Draw Has Started!", prize.Name, customData));
            }

            //Do not await this, just fire and forget
            Task.WhenAll(notificationTaskList);

            return Ok(winner);
        }

        [HttpGet("prize/{id}")]
        public async Task<ActionResult<PrizeResultDTO>> GetPrizeResult(string id)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var survey = await _context.Surveys.FindAsync(id);
            var prizes = await _context.Prizes.Where(x => x.SurveyId == id).ToListAsync();

            var groupList = await TryGetGroups(user.Value);
            var groupIds = groupList.Select(x => x.Id);

            var group = groupList.Where(x => x.Id == survey.GroupId).FirstOrDefault();

            //TODO: Force admin
            if (group == null)// || group.IsAdmin == false)
                return BadRequest();


            var surveyAnswers = from a in _context.Answers
                                  join q in _context.Questions on a.QuestionId equals q.Id
                                  join u in _context.UserAccounts on a.UserAccountId equals u.Id
                                  where q.SurveyId == survey.Id
                                  select new { Question = q, UserAccount = u, Answer = a };


            //var questionResults = surveyAnswers.GroupBy(x => x.Question.Name)
            //    .Select(x => new QuestionResultDTO()
            //    {
            //        Name = x.Key,
            //        AverageRating = x.Average(y => y.Answer.Rating),
            //        Ratings = x.Select(y => y.Answer.Rating).ToList(),
            //        Comments = x.Select(y => y.Answer.Comment).ToList()
            //    }).ToList();

            var entries = surveyAnswers.GroupBy(x => x.UserAccount.Id)
            .Select(x => new WinnerDTO()
            {
                 UserAccountId = x.Key,
                 Name = x.FirstOrDefault().UserAccount.Name
            });

            


            var prizeResultDTO = new PrizeResultDTO()
            {
                SurveyName = survey.Name,
                DatePublished = (DateTimeOffset)survey.DatePublished,
                Image = group.KeyPhoto,
                Prizes = new List<PrizeDTO>(),
                EntryCount = entries.Count()
                //EntryCount = questionResults.FirstOrDefault() != null ? questionResults.FirstOrDefault().Ratings.Count : 0
            };

            foreach (var prize in prizes)
            {
                var prizeDTO = ToDTO(prize);
                if (prize.UserAccountId != null)
                {
                    var winner = _context.UserAccounts
                        .Where(x => x.Id == prize.UserAccountId)
                        .FirstOrDefault();
                    prizeDTO.WinnerName = winner.Name;
                    prizeDTO.WinnerUserAccountId = winner.Id;
                    prizeDTO.WinnerPhoto = winner.Photo;
                    prizeDTO.Photo = prize.Photo;
                }
                prizeResultDTO.Prizes.Add(prizeDTO);
            }

            //If Is Admin
            prizeResultDTO.Entries = entries.ToList();

            return prizeResultDTO;
        }

        //public async Task<List<GroupDTO>> GetGroupList(UserAccount user)
        //{
        //    List<string> adminList = null;
        //    if (this.appSettings.Value.AdminOverrideList.Length > 0)
        //    {
        //        adminList = this.appSettings.Value.AdminOverrideList.Split(';').ToList();
        //    }

        //    List<GroupDTO> groupList = new List<GroupDTO>();

        //    var groups = await meetupClient.GetGroups(this.Token, new MyQueryParams() { fields = "self" });

        //    foreach (var group in groups)
        //    {
        //        var dto = Helpers.ToDTO(group);

        //        if (adminList != null && adminList.Contains(user.Email))
        //            dto.IsAdmin = true;

        //        groupList.Add(dto);
        //    }

        //    return groupList;
        //}

        //public async Task<GroupDTO> GetGroup(UserAccount user, string groupId)
        //{
        //    var groups = await GetGroupList(user);
        //    var group = groups.Where(x => x.Id == groupId).FirstOrDefault();

        //    return group;
        //}


    }
}
