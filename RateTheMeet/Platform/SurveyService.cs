using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;

namespace RateTheMeet.Platform
{
    public class SurveyService : ISurveyService
    {
        readonly ApiClientFactory apiClient;

        readonly Subject<Unit> groupsRefreshedSubject;
        public IObservable<Unit> GroupsRefreshed() => this.groupsRefreshedSubject;

        readonly Subject<Unit> surveysRefreshedSubject;
        public IObservable<Unit> SurveysRefreshed() => this.surveysRefreshedSubject;

        readonly Subject<bool> overrideAdminSubject;
        public IObservable<bool> AdminOverrided() => this.overrideAdminSubject;

        public bool OverrideAdmin { get; private set; }
        public bool AdminOfAnyGroup { get; private set; }

        public SurveyService(ApiClientFactory factory)
        {
            this.apiClient = factory;
            surveysRefreshedSubject = new Subject<Unit>();
            groupsRefreshedSubject = new Subject<Unit>();
            overrideAdminSubject = new Subject<bool>();
        }

        public void SetAdminOverride(bool isOn)
        {
            OverrideAdmin = isOn;
            overrideAdminSubject.OnNext(isOn);
        }

        public Task<AuthUser> RefreshProfile()
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> AddSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public Task PublishSurvey(string id)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveSurvey(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyDTO>> GetSurveys(bool forceUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyDTO>> GetArchivedSurveys()
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> GetSurvey(string surveyId)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> UpdateSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSurvey(string surveyId)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyResultDTO> GetSurveyResult(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PrizeResultDTO> GetPrizeResult(string id)
        {
            throw new NotImplementedException();
        }

        public Task<WinnerDTO> StartPrizeDraw(string prizeId)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadPrizePhoto(string prizeId, Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task<List<AnswerDTO>> SubmitAnswers(List<AnswerDTO> answers)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupDTO>> GetGroups(bool forceUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSurveysCompleted()
        {
            throw new NotImplementedException();
        }

        public Task<List<QuestionDTO>> GetPreviousQuestions()
        {
            throw new NotImplementedException();
        }

        public Task PublishSurvey(PublishSurveyArgs publishArgs)
        {
            throw new NotImplementedException();
        }
    }
}
