using System;
using System.Threading.Tasks;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;

namespace MeetupSurvey.Core
{
    public enum SignOutReason
    {
        UserLoggingOut,
        TokenExpired
    }


    public interface IProfile
    {
        Task<AuthUser> GetUser();
        Task SignIn(AuthUser userAccount);
        Task SignOut(SignOutReason reason);
        void ProfileError(string error);
        Task<AuthUser> UpdateToken(AuthUser user, AuthToken token);
        Task UpdateProfile(AuthUser currentUser, AuthUser updatedUser);
        Task Refresh(AuthUser userAccount);


        IObservable<AuthUser> WhenUserSignedIn();
        IObservable<(AuthUser User, SignOutReason Reason)> WhenUserSignedOut();
        IObservable<string> WhenProfileError();
    }
}
