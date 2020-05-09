using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MeetupSurvey.ApiClient;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Data.Impl
{
    public class MeetupAuthService : IMeetupService
    {
        private readonly IAppSettings appSettings;
        public MeetupAuthService(ApiClientFactory meetupClient, IAppSettings settings)
        {
            this.meetupClient = meetupClient;
            this.appSettings = settings;
        }

        ApiClientFactory meetupClient;
        public async Task<AuthUser> Authenticate(string code)
        {
            try
            {
                var response = await this.meetupClient
                    .Instance
                    .Authenticate(new MeetupAccessArgs()
                    {
                        client_id = appSettings.MeetupClientId,
                        grant_type = appSettings.MeetupGrantType,
                        redirect_uri = appSettings.MeetupRedirectUri,
                        code = code
                    })
                    .ConfigureAwait(false);

                return response;
            }
            catch (Refit.ApiException ex)
            {
                return null;
            }
        }

        public async Task<AuthToken> RefreshToken(AuthUser user)
        {
            try
            {
                var response = await this.meetupClient
                    .Instance
                    .RefreshToken(new MeetupRefreshArgs()
                    {
                        client_id = appSettings.MeetupClientId,
                        grant_type = appSettings.MeetupRefreshGrantType,
                        refresh_token = user.refresh_token
                    })
                    .ConfigureAwait(false);
                                    return response;
            }
            catch (Refit.ApiException ex)
            {
                return null;
            }
        }


    }
}
