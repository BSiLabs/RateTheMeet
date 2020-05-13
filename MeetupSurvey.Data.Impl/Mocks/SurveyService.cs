using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Data.Impl.Mocks
{
    public class SurveyService : ISurveyService
    {
        private List<QuestionDTO> _questions;

        private List<SurveyDTO> _surveys;

        private List<GroupDTO> _groups;

        public SurveyService()
        {
            _groups = new List<GroupDTO>
            {
                new GroupDTO
                {
                    Id = "1",
                    Name = "Test Group 1",
                    Description = "Test Group 1 Desc",
                    Link = "https://testgroup1.com",
                    GroupPhoto = "https://picsum.photos/200",
                    KeyPhoto = "https://picsum.photos/200",
                    NextEventId = "1",
                    EventName = "Test Event 1",
                    EventTime = "Febtober",
                    EventLink = "https://testgroup1.com/events/1",
                    EventRsvp = 10,
                    IsAdmin = true,
                    //Organizers = ,
                    UrlName = "Test Group 1 Url",
                    GroupSubscribed = true
                }
            };
            
            _questions = new List<QuestionDTO>
            {
                new QuestionDTO
                {
                    Id = "1",
                    Name = "S1 Q1",
                    Order = 1,
                    SurveyId = "1",
                    Deleted = false
                },
                new QuestionDTO
                {
                    Id = "2",
                    Name = "S1 Q2",
                    Order = 1,
                    SurveyId = "1",
                    Deleted = false
                },
                new QuestionDTO
                {
                    Id = "3",
                    Name = "S2 Q1",
                    Order = 1,
                    SurveyId = "2",
                    Deleted = false
                },
                new QuestionDTO
                {
                    Id = "4",
                    Name = "S2 Q2",
                    Order = 1,
                    SurveyId = "2",
                    Deleted = false
                }
            };
            
            _surveys = new List<SurveyDTO>
            {
                new SurveyDTO
                {
                    Id = "1",
                    Name = "Test Survey 1",
                    HasCompleted = true,
                    DatePublished = DateTimeOffset.Now.AddDays(-2),
                    Archived = false,
                    Average = 5,
                    Entries = 10,
                    GroupName = "Test Group",
                    GroupId = "1",
                    Group = new GroupDTO
                    {
                        IsAdmin = true,
                    },
                    Prizes = new List<PrizeDTO>(),
                    Questions = _questions.Where(x => x.SurveyId == "1").ToList()
                },
                new SurveyDTO
                {
                    Id = "2",
                    Name = "Test Survey 2",
                    HasCompleted = false,
                    DatePublished = DateTimeOffset.Now.AddDays(-1),
                    Archived = false,
                    Average = 5,
                    Entries = 10,
                    GroupName = "Test Group",
                    GroupId = "1",
                    Group = new GroupDTO
                    {
                        IsAdmin = true,
                    },
                    Prizes = new List<PrizeDTO>(),
                    Questions = _questions.Where(x => x.SurveyId == "2").ToList()
                }
            };
        }

        public bool OverrideAdmin => throw new NotImplementedException();

        public bool AdminOfAnyGroup => throw new NotImplementedException();

        public Task<SurveyDTO> AddSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> AdminOverrided()
        {
            throw new NotImplementedException();
        }

        public Task ArchiveSurvey(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSurvey(string surveyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyDTO>> GetArchivedSurveys()
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupDTO>> GetGroups(bool forceUpdate) => Task.FromResult(_groups);

        public Task<List<QuestionDTO>> GetPreviousQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<PrizeResultDTO> GetPrizeResult(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> GetSurvey(string surveyId)
        {
            return Task.FromResult(_surveys.Single(x => x.Id == surveyId));
        }

        public Task<SurveyResultDTO> GetSurveyResult(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyDTO>> GetSurveys(bool forceUpdate)
        {
            return Task.FromResult(_surveys);
        }

        public Task<string> GetSurveysCompleted()
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> GroupsRefreshed()
        {
            throw new NotImplementedException();
        }

        public Task PublishSurvey(PublishSurveyArgs publishArgs)
        {
            throw new NotImplementedException();
        }

        public Task<AuthUser> RefreshProfile()
        {
            throw new NotImplementedException();
        }

        public void SetAdminOverride(bool isOn)
        {
            throw new NotImplementedException();
        }

        public Task<WinnerDTO> StartPrizeDraw(string prizeId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AnswerDTO>> SubmitAnswers(List<AnswerDTO> answers)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> SurveysRefreshed()
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> UpdateSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadPrizePhoto(string prizeId, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
