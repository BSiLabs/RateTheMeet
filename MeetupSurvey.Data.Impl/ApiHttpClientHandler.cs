using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MeetupSurvey.ApiClient;
using MeetupSurvey.Core;
using MeetupSurvey.DTO;
using Refit;
using Xamarin.Essentials;

namespace MeetupSurvey.Data.Impl
{
    public class ApiHttpClientHandler : DelegatingHandler
    {
        readonly IApiClient apiClient;
        readonly IProfile profile;
        readonly IAppSettings appSettings;
        bool retried = false;


        public ApiHttpClientHandler(IProfile profile, IAppSettings appSettings)
        {
            this.profile = profile;
            this.appSettings = appSettings;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
                var user = await this.profile.GetUser();
                if (user != null)
                {
                    //var client = this.GetClient();
                    var accessToken = user.access_token;
                    if (DateTime.UtcNow > user.Expiry)
                    {
                        await RefreshToken(user);
                    }

                    request.Headers.Authorization = new AuthenticationHeaderValue(
                        "bearer",
                        accessToken
                    );
                }
      
                var response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !retried)
                {
                    await RefreshToken(user);
                    retried = true;

                    return await SendAsync(request, cancellationToken);
                }
                else
                {
                    retried = false;
                    return response;
                }
        }


        IApiClient GetClient() => RestService.For<IApiClient>(this.appSettings.BaseApiUri);


        private async Task RefreshToken(AuthUser user)
        {
                var client = this.GetClient();

                var newToken = await client.RefreshToken(new MeetupRefreshArgs
                {
                    client_id = appSettings.MeetupClientId,
                    grant_type = appSettings.MeetupRefreshGrantType,
                    refresh_token = user.refresh_token
                });

                await this.profile.UpdateToken(user, newToken);
        }
    }
}
