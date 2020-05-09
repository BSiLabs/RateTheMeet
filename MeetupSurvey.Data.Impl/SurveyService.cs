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
using MonkeyCache.SQLite;

namespace MeetupSurvey.Data.Impl
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

        public async Task<AuthUser> RefreshProfile()
        {
            try
            {
                var response = await this.apiClient
                    .Instance
                    .RefreshProfile()
                    .ConfigureAwait(false);
                return response;
            }
            catch (Refit.ApiException ex)
            {
                return null;
            }
        }

        public async Task ClearCache()
        {
            try
            {
                await this.apiClient
                    .Instance
                    .ClearCache()
                    .ConfigureAwait(false);
            }
            catch (Refit.ApiException ex)
            {
                throw ex;
            }
        }

        #region Surveys
        public async Task<List<SurveyDTO>> GetSurveys(bool forceUpdate = false)
        {
            var key = "Surveys";
            List<SurveyDTO> surveys;

            if (!Barrel.Current.IsExpired(key) && Barrel.Current.Exists(key) && !forceUpdate)
            {
                surveys = Barrel.Current.Get<List<SurveyDTO>>(key);
            }
            else
            {
               surveys = await this.apiClient
                                    .Instance
                                    .GetSurveys()
                                    .ConfigureAwait(false);

                Barrel.Current.Add(key: key, data: surveys, expireIn: TimeSpan.FromHours(5));
            }

            if (OverrideAdmin)
                foreach (var survey in surveys)
                    survey.Group.IsAdmin = true;
            

            surveysRefreshedSubject.OnNext(Unit.Default);

            return surveys;
        }

        public async Task<SurveyDTO> AddSurvey(SurveyDTO survey)
        {
            try
            {
                var response = await this.apiClient
                    .Instance
                    .AddSurvey(survey)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Refit.ApiException ex)
            {
                return null;
            }
        }

        public async Task DeleteSurvey(string surveyId)
        {
            try
            {
                await this.apiClient
                    .Instance
                    .DeleteSurvey(surveyId)
                    .ConfigureAwait(false);
            }
            catch (Refit.ApiException ex)
            {
                throw;
            }
        }

        public async Task<SurveyDTO> GetSurvey(string surveyId)
        {
            try
            {
                var survey = await this.apiClient
                .Instance
                .GetSurvey(surveyId)
                .ConfigureAwait(false);

                if (OverrideAdmin)
                    survey.Group.IsAdmin = true;

                return survey;
            }
            catch (Refit.ApiException ex)
            {
                throw;
            }
        }

        public async Task<SurveyDTO> UpdateSurvey(SurveyDTO survey)
        {
            try
            {
                var response = await this.apiClient
                    .Instance
                    .UpdateSurvey(survey)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Refit.ApiException ex)
            {
                throw;
            }
        }

        public async Task<SurveyResultDTO> GetSurveyResult(string surveyId)
        {
            try
            {
                    var surveyResult = await this.apiClient
                    .Instance
                    .GetSurveyResult(surveyId)
                    .ConfigureAwait(false);

                return surveyResult;
            }
            catch (Refit.ApiException ex)
            {
                throw ex;
            }
        }

        public async Task<PrizeResultDTO> GetPrizeResult(string surveyId)
        {
            try
            {
                var key = "PrizeResult";
                PrizeResultDTO prizeResult;

                if (!Barrel.Current.IsExpired(key) && Barrel.Current.Exists(key))
                {
                    prizeResult = Barrel.Current.Get<PrizeResultDTO>(key);
                }
                else
                {
                    prizeResult = await this.apiClient
                    .Instance
                    .GetPrizeResult(surveyId)
                    .ConfigureAwait(false);
                }
                return prizeResult;
            }
            catch (Refit.ApiException ex)
            {
                throw;
            }
        }

        public async Task<List<AnswerDTO>> SubmitAnswers(List<AnswerDTO> answers)
        {
            try
            {
                var response = await this.apiClient
                    .Instance
                    .SubmitAnswers(answers)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Refit.ApiException ex)
            {
                throw;
            }
        }

        #endregion

        #region Groups

        public async Task<List<GroupDTO>> GetGroups(bool forceUpdate = false)
        {
            var key = "Groups";
            List<GroupDTO> groups;

            if (!Barrel.Current.IsExpired(key) && Barrel.Current.Exists(key) && !forceUpdate)
            {
                groups = Barrel.Current.Get<List<GroupDTO>>(key);
            }
            else
            {
                groups = await this.apiClient
                                   .Instance
                                   .GetGroups()
                                   .ConfigureAwait(false);

                Barrel.Current.Add(key: key, data: groups, expireIn: TimeSpan.FromHours(5));
            }

            // TODO Comment out when done with toggle switch
            if (OverrideAdmin)
            {
                foreach (var g in groups)
                {
                    g.IsAdmin = true;
                }
            }

            AdminOfAnyGroup = groups.Any(x => x.IsAdmin);

            groupsRefreshedSubject.OnNext(Unit.Default);
            return groups;
        }

        public async Task<string> GetSurveysCompleted()
        {

            var surveys = await this.apiClient
                .Instance
                .GetSurveysCompleted()
                .ConfigureAwait(false);

            return surveys;
        }

        public async Task PublishSurvey(PublishSurveyArgs publishArgs)
        {
            await this.apiClient
                .Instance
                .PublishSurvey(publishArgs)
                .ConfigureAwait(false);

        }

        public async Task ArchiveSurvey(string id)
        {
            await this.apiClient
                .Instance
                .ArchiveSurvey(id)
                .ConfigureAwait(false);
        }

        public async Task<List<SurveyDTO>> GetArchivedSurveys()
        {
            var key = "ArchivedSurveys";
            List<SurveyDTO> archivedSurveys;

            if (!Barrel.Current.IsExpired(key) && Barrel.Current.Exists(key))
            {
                archivedSurveys = Barrel.Current.Get<List<SurveyDTO>>(key);
            }
            else
            {
                archivedSurveys = await this.apiClient
                                            .Instance
                                            .GetArchivedSurveys()
                                            .ConfigureAwait(false);
            }

            return archivedSurveys;
        }

        public async Task<WinnerDTO> StartPrizeDraw(string prizeId)
        {
            var winner =  await this.apiClient
                .Instance
                .StartPrizeDraw(prizeId)
                .ConfigureAwait(false);

            return winner;
        }

        public async Task<string> UploadPrizePhoto(string prizeId, Stream stream)
        {
            return await this.apiClient
                .Instance
                .UploadPhoto(prizeId, new Refit.StreamPart(stream, Guid.NewGuid() + ".jpeg", "image/jpeg"))
                .ConfigureAwait(false);

        }

        public async Task<List<QuestionDTO>> GetPreviousQuestions()
        {
            return await this.apiClient
                .Instance
                .GetPreviousQuestions()
                .ConfigureAwait(false);
        }

        #endregion

        #region Answers


        #endregion
    }
}
