using System;
using System.Threading.Tasks;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Data.Impl.Mocks
{
    public class AccountService : IAccountService
    {
        readonly IProfile profile;

        public AccountService(IProfile profile)
        {
            this.profile = profile;
        }

        public string Code { set => throw new NotImplementedException(); }

        public async Task<bool> Authenticate(string token)
        {
            var user = new AuthUser();
            await profile.SignIn(user);

            return true;
        }

        public Task<bool> Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<AuthToken> GetToken()
        {
            throw new NotImplementedException();
        }

        public Task<AuthUser> GetUser()
        {
            throw new NotImplementedException();
        }

        public void ProcessUri(Uri code)
        {
            throw new NotImplementedException();
        }

        public Task SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
