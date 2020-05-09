using MeetupSurvey.DTO;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Services
{
    public interface IMeetupAuth
    {
        ///*===========AUTH============*/
        [Post("/access")]
        Task<AuthTokenResponse> Access(string client_id, string client_secret, string grant_type, string redirect_uri, string code);

        [Post("/access")]
        Task<AuthTokenResponse> Access(string client_id, string client_secret, string grant_type, string refresh_token);
    }
}
