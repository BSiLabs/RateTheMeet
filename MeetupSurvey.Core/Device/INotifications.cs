using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetupSurvey.Core.Device
{
    public interface INotifications
    {
        void OpenSettings();
        Task<NotificationState> GetStatus();
        Task<bool> RequestPermission();
        Task<string> GetPushId();
        Task CancelAll();
        Task Send(string title, string message, IDictionary<string, string> customData);

        IObservable<MeetupSurvey.DTO.Notification> WhenPrizeNotified();
        IObservable<MeetupSurvey.DTO.Notification> WhenNewSurvey();
    }
}
