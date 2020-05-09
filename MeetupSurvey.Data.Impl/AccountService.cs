using System;
using System.Threading.Tasks;
using MeetupSurvey.ApiClient;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using MonkeyCache.SQLite;
//using Xamarin.Auth;
using Xamarin.Forms;

namespace MeetupSurvey.Data.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IAppSettings appSettings;
        private readonly IProfile profile;
        private readonly IAppSettings settings;

        public AccountService(ApiClientFactory meetupClient, IAppSettings settings, IProfile profile)
        {
            this.meetupClient = meetupClient;
            this.appSettings = settings;
            this.profile = profile;
            this.settings = settings;
        }

//        public OAuth2Authenticator MeetupAuth { get; set; }

        public string Code { get; set; }

        public async Task<bool> Authenticate()
        {
            try
            {

                //Xamarin.Auth method

                //this.MeetupAuth = new OAuth2Authenticator(
                //settings.MeetupClientId,
                //null,
                //"",
                //new Uri(settings.MeetupAuthUri),
                //new Uri(settings.MeetupRedirectUri),
                //new Uri(settings.MeetupTokenUri),
                //null,
                //true);
                //this.MeetupAuth.AllowCancel = true;

                //MeetupAuth.Completed += async (sender, eventArgs) =>
                //{
                //    var user = await GetUser();
                //    if (user == null)
                //        profile.ProfileError("There was an error signing you in, please try again.");
                //    else
                //        await profile.SignIn(user);
                //    //TODO: Do something with token
                //};
                //MeetupAuth.Error += (sender, EventArgs) =>
                //{

                //};

                //var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                //presenter.Login(MeetupAuth);
                ///////////////////////////

                //Manual browser method
                await Xamarin.Essentials.Browser.OpenAsync(settings.MeetupAuthUri + "?client_id=" + settings.MeetupClientId + "&response_type=code&redirect_uri=" +settings.MeetupRedirectUri + "&suppress=reg");
                ///////////


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async void ProcessUri(Uri uri)
        {
            var code = uri.ToString().Split('?')[1]
                               .Split('&')[0]
                               .Split('=')[1];

            var hashIndex = code.IndexOf("#", StringComparison.Ordinal);
            this.Code = hashIndex > -1 ? code.Substring(0, hashIndex) : code;

            //Manual browser method
            var user = await GetUser();
            await profile.SignIn(user);
            /////////////


            //Xam.Auth method
            //MeetupAuth.OnCancelled();
            /////////////
        }

        public async Task SignOut()
        {
            Barrel.Current.EmptyAll();
            await Xamarin.Essentials.Browser.OpenAsync("http://www.meetup.com/logout");
            //Add an artifical delay to ensure that logout completes in the browser
            //await Task.Delay(4000);
        }   

        ApiClientFactory meetupClient;
        public async Task<AuthUser> GetUser()
        {
            try
            {
                if (Code != null)
                {
                    var response = await this.meetupClient
                    .Instance
                    .Authenticate(new MeetupAccessArgs()
                    {
                        client_id = appSettings.MeetupClientId,
                        grant_type = appSettings.MeetupGrantType,
                        redirect_uri = appSettings.MeetupRedirectUri,
                        code = Code
                    })
                    .ConfigureAwait(false);

                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


    }
}
