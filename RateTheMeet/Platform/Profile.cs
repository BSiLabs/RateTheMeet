using MeetupSurvey.Core;
using MeetupSurvey.DTO;
using System;
using System.Threading.Tasks;

namespace RateTheMeet.Platform
{
    public class Profile : IProfile
    {
        public Task<AuthUser> GetUser()
        {
            throw new NotImplementedException();
        }

        public void ProfileError(string error)
        {
            throw new NotImplementedException();
        }

        public Task Refresh(AuthUser userAccount)
        {
            throw new NotImplementedException();
        }

        public Task SignIn(AuthUser userAccount)
        {
            throw new NotImplementedException();
        }

        public Task SignOut(SignOutReason reason)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProfile(AuthUser currentUser, AuthUser updatedUser)
        {
            throw new NotImplementedException();
        }

        public Task<AuthUser> UpdateToken(AuthUser user, AuthToken token)
        {
            throw new NotImplementedException();
        }

        public IObservable<string> WhenProfileError()
        {
            throw new NotImplementedException();
        }

        public IObservable<AuthUser> WhenUserSignedIn()
        {
            throw new NotImplementedException();
        }

        public IObservable<(AuthUser User, SignOutReason Reason)> WhenUserSignedOut()
        {
            throw new NotImplementedException();
        }
    }
}
