using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using Refit;

namespace MeetupSurvey.ApiClient
{
    public interface IApiClient
    {
        ///*===========AUTH============*/
        [Post("/MeetupAuth/Authorize")]
        Task<AuthUser> Authenticate([Body] MeetupAccessArgs args);

        [Post("/MeetupAuth/RefreshToken")]
        Task<AuthToken> RefreshToken([Body] MeetupRefreshArgs args);

        [Get("/UserAccounts/RefreshProfile")]
        Task<AuthUser> RefreshProfile();

        [Post("/UserAccounts/ClearCache")]
        Task ClearCache();



        ///*===========SURVEYS============*/
        [Get("/surveys")]
        Task<List<SurveyDTO>> GetSurveys();
        [Get("/surveys/archived")]
        Task<List<SurveyDTO>> GetArchivedSurveys();

        [Get("/surveys/{id}")]
        Task<SurveyDTO> GetSurvey(string id);

        [Post("/surveys")]
        Task<SurveyDTO> AddSurvey([Body]SurveyDTO survey);

        [Put("/surveys")]
        Task<SurveyDTO> UpdateSurvey([Body] SurveyDTO survey);


        [Post("/surveys/publish")]
        Task PublishSurvey([Body] PublishSurveyArgs publishArgs);

        [Get("/surveys/archive/{id}")]
        Task ArchiveSurvey(string id);



        [Delete("/surveys/{id}")]
        Task DeleteSurvey(string id);

        [Get("/surveys/completed")]
        Task<string> GetSurveysCompleted();

        [Get("/surveys/result/{id}")]
        Task<SurveyResultDTO> GetSurveyResult(string id);

        [Get("/surveys/prize/{id}")]
        Task<PrizeResultDTO> GetPrizeResult(string id);

        [Get("/surveys/prize/startdraw/{prizeId}")]
        Task<WinnerDTO> StartPrizeDraw(string prizeId);

        ///*===========QUESTIONS============*/
        [Get("/questions/previous")]
        Task<List<QuestionDTO>> GetPreviousQuestions();

        ///*===========GROUPS============*/
        [Get("/groups")]
        Task<List<GroupDTO>> GetGroups();


        ///*===========ANSWERS============*/
        [Post("/answers/submit")]
        Task<List<AnswerDTO>> SubmitAnswers([Body]List<AnswerDTO> answers);

        ///*===========Prizes============*/
        [Multipart]
        [Post("/prizes/{id}/photo")]
        Task<string> UploadPhoto(string id, [AliasAs("file")]StreamPart stream);


        [Post("/push/registration")]
        Task Registration([Body] PushRegisterArgs args);
    }
}
