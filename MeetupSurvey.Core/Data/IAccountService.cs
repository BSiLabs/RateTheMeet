using System;
using System.Threading.Tasks;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Core.Data
{
    public interface IAccountService
    {
        string Code { set; }
        Task<bool> Authenticate();
        Task SignOut();
        Task<AuthUser> GetUser();
        void ProcessUri(Uri uri);
    }
}
