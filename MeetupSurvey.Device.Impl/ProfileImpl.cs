using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace MeetupSurvey.Device.Impl
{
    public class ProfileImpl : IProfile
    {
        AuthUser userAccount;

        const string USER_KEY = "User";
        readonly Subject<AuthUser> signInSubject;
        readonly Subject<(AuthUser User, SignOutReason Reason)> signOutSubject;

        readonly Subject<string> profileErrorSubject;


        public ProfileImpl()
        {
            this.signInSubject = new Subject<AuthUser>();
            this.signOutSubject = new Subject<(AuthUser User, SignOutReason Reason)>();
            this.profileErrorSubject = new Subject<string>();
        }


        public IObservable<AuthUser> WhenUserSignedIn() => this.signInSubject;
        public IObservable<(AuthUser User, SignOutReason Reason)> WhenUserSignedOut() => this.signOutSubject;
        public IObservable<string> WhenProfileError() => this.profileErrorSubject;




        public async Task SignIn(AuthUser userAccount)
        {
            if (userAccount == null)
                throw new ArgumentException("Login failed");

            await this.Store(userAccount);
            this.signInSubject.OnNext(userAccount);
        }


        public async Task Refresh(AuthUser userInfo)
        {
            var user = await this.GetUser();
            if (user == null)
                throw new ArgumentException("Cannot refresh user since user is not authenticated");

            await this.Store(userInfo);
        }


        public async Task SignOut(SignOutReason reason)
        {
            var user = await this.GetUser();
            this.userAccount = null;
            SecureStorage.Remove(USER_KEY);
            this.signOutSubject.OnNext((user, reason));
        }

        public void ProfileError(string error)
        {
            this.profileErrorSubject.OnNext(error);
        }


        async Task Store(AuthUser user)
        {
            this.userAccount = user;
            var value = JsonConvert.SerializeObject(user);
            await SecureStorage.SetAsync(USER_KEY, value);
        }
        public async Task<AuthUser> GetUser()
        {
            if (this.userAccount != null)
              return this.userAccount;

            try
            {
                var s = await SecureStorage.GetAsync(USER_KEY);
                if (s != null)
                {
                    var tmp = JsonConvert.DeserializeObject<AuthUser>(s);

                    this.userAccount = tmp;
                }
            }
            catch
            {
                // deals with an android issue that can occur with online backups
                SecureStorage.Remove(USER_KEY);
            }
            return this.userAccount;
        }




        public async Task<AuthUser> UpdateToken(AuthUser user, AuthToken token)
        {
            user.access_token = token.access_token;
            user.refresh_token = token.refresh_token;
            user.Expiry = token.expiry;
            user.token_type = token.token_type;
            await Refresh(user);
            return user;
        }


        public async Task UpdateProfile(AuthUser currentUser, AuthUser updatedUser)
        {
            currentUser.Name = updatedUser.Name;
            currentUser.Email = updatedUser.Email;
            currentUser.Photo = updatedUser.Photo;
            await Refresh(currentUser);
        }

    }
}
