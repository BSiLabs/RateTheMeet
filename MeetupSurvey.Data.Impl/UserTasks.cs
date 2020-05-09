using System;
using System.Threading.Tasks;
using Autofac;
using MeetupSurvey.ApiClient;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Core.Device;
using MeetupSurvey.DTO;
using Refit;
using Xamarin.Essentials;

namespace MeetupSurvey.Data.Impl
{
    public class UserTasks : IStartable
    {
        readonly IProfile profile;
        readonly ApiClientFactory apiClient;
        readonly IAppSettings appSettings;
        readonly ILogger logger;
        readonly INetwork network;
        readonly INotifications notifications;


        public UserTasks(IProfile profile,
                         ApiClientFactory apiClient,
                         IAppSettings appSettings,
                         INetwork network,
                         INotifications notifications,
                         ILogger logger)
        {
            this.profile = profile;
            this.apiClient = apiClient;
            this.appSettings = appSettings;
            this.network = network;
            this.notifications = notifications;
            this.logger = logger;
        }


        public void Start()
        {
            this.profile.WhenUserSignedIn().Subscribe(x =>
                this.Registration(x, false)
            );
            this.profile.WhenUserSignedOut().Subscribe(p =>
            {
                //this.KillAuth(p.User);
                this.Registration(p.User, true);
            });
        }


        async void Registration(AuthUser user, bool unregister)
        {
            try
            {
                var pushId = await this.notifications.GetPushId();

                await RestService
                    .For<IPushApiClient>(this.appSettings.BaseApiUri)
                    .Registration(new PushRegisterArgs
                    {
                        IsUnRegister = unregister,
                        Email = user.Email,
                        InstallId = pushId,
                        Jwt = user.access_token,
                        IsAndroid = DeviceInfo.Platform == DevicePlatform.Android
                    });
            }
            catch (Exception ex)
            {
                this.logger.WriteCrash(ex);
            }
        }


        //async void KillAuth(AuthUser user)
        //{
        //    try
        //    {
        //        if (!this.network.IsAvailable)
        //            return;

        //        var token = user.access_token;
        //        await apiClient.Instance.SignOut("bearer " + token);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.WriteCrash(ex);
        //    }
        //}
    }
}
