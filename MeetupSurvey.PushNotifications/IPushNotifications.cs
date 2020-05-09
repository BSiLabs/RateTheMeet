using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetupSurvey.Services.PushNotifications
{
    public interface IPushNotifications
    {
        Task SendAll(string title, string message);
        Task Send(List<Device> devices, string title, string message, IDictionary<string, string> customData);
        Task Send(Device device, string title, string message, IDictionary<string, string> customData);
    }
}
