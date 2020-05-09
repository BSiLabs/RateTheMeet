using System;
using System.Threading.Tasks;
using MeetupSurvey.DTO;
using Refit;

namespace MeetupSurvey.ApiClient
{
    public interface IPushApiClient
    {
        [Post("/push/registration")]
        Task Registration([Body] PushRegisterArgs args);
    }
}
