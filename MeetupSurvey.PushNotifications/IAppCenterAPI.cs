using System;
using System.Threading.Tasks;
using Refit;

namespace MeetupSurvey.Services.PushNotifications
{
    public interface IAppCenterApi
    {
        [Post("/v0.1/apps/{ownerName}/{appName}/push/notifications")]
        Task Push([Header("X-API-Token")] string apiKey, string ownerName, string appName, [Body] AppCenterNotification notification);
    }
}
