using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Core.Data
{
    public interface ISurveyService
    {
        bool OverrideAdmin { get; }
        bool AdminOfAnyGroup { get; }
        IObservable<bool> AdminOverrided();
        void SetAdminOverride(bool isOn);

        IObservable<Unit> GroupsRefreshed();
        IObservable<Unit> SurveysRefreshed();
        Task<AuthUser> RefreshProfile();

        Task<SurveyDTO> AddSurvey(SurveyDTO survey);
        Task PublishSurvey(PublishSurveyArgs publishArgs);
        Task ArchiveSurvey(string id);
        Task<List<SurveyDTO>> GetSurveys(bool forceUpdate);
        Task<List<SurveyDTO>> GetArchivedSurveys();
        Task<SurveyDTO> GetSurvey(string surveyId);

        Task<SurveyDTO> UpdateSurvey(SurveyDTO survey);
        Task DeleteSurvey(string surveyId);

        Task<SurveyResultDTO> GetSurveyResult(string id);
        Task<PrizeResultDTO> GetPrizeResult(string id);
        Task<WinnerDTO> StartPrizeDraw(string prizeId);
        Task<string> UploadPrizePhoto(string prizeId, Stream stream);

        Task<List<AnswerDTO>> SubmitAnswers(List<AnswerDTO> answers);


        Task<List<GroupDTO>> GetGroups(bool forceUpdate);
        Task<string> GetSurveysCompleted();

        Task<List<QuestionDTO>> GetPreviousQuestions();
    }
}
