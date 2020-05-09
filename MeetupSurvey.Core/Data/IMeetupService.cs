using System;
using System.Threading.Tasks;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Core.Data
{

    public interface IMeetupService
    {
        Task<AuthUser> Authenticate(string code);
        Task<AuthToken> RefreshToken(AuthUser user);
    }

}
