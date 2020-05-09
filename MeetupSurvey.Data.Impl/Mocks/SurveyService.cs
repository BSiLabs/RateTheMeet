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
        public SurveyService()
        {
        }

        public async Task<List<SurveyDTO>> GetSurveys(bool forceUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> GetSurvey(string surveyId)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetSurveyQuestions(string surveyId)
        {
            throw new NotImplementedException();
        }


        public Task DeleteSurvey(string surveyId)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> AddSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDTO> UpdateSurvey(SurveyDTO survey)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupDTO>> GetGroups(bool forceUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<List<AnswerDTO>> SubmitAnswers(List<AnswerDTO> answers)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSurveysCompleted()
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

        public Task<AuthUser> RefreshProfile()
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

        public Task<List<SurveyDTO>> GetArchivedSurveys()
        {
            throw new NotImplementedException();
        }

        public bool OverrideAdmin { get; set; }

        public bool AdminOfAnyGroup => throw new NotImplementedException();

        public IObservable<Unit> SurveysRefreshed()
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> AdminOverrided()
        {
            throw new NotImplementedException();
        }

        public void SetAdminOverride(bool isOn)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> GroupsRefreshed()
        {
            throw new NotImplementedException();
        }

        public Task<WinnerDTO> StartPrizeDraw(string prizeId)
        {
            throw new NotImplementedException();
        }

        public Task UploadPrizePhoto(string prizeId, Stream stream)
        {
            throw new NotImplementedException();
        }

        Task<string> ISurveyService.UploadPrizePhoto(string prizeId, Stream stream)
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
