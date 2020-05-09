using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace MeetupSurvey.Services.PushNotifications
{
    // API: https://docs.microsoft.com/en-us/appcenter/push/rest-api
    // Mobile Side: https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-forms
    public class AppCenterPushNotifications : IPushNotifications
    {
        public string BaseUri { get; set; } = "https://api.appcenter.ms";
        public string ApiKey { get; set; } = "";
        public string OrganizationName { get; set; } = "";

        public string iOSAppName { get; set; }
        public string AndroidAppName { get; set; }

        public AppCenterPushNotifications()
        {
        }


        public async Task Send(List<Device> devices, string title, string message, IDictionary<string, string> customData)
        {
            var taskList = devices.Select(device => this.Send(device, title, message, customData));
            await Task.WhenAll(taskList);
        }


        public async Task SendAll(string title, string message)
        {
            var apiClient = RestService.For<IAppCenterApi>(this.BaseUri);
            var notification = new AppCenterNotification
            {
                Content = new NotificationContent
                {
                    Title = title,
                    Body = message
                }
            };
            await Task.WhenAll
            (
                apiClient.Push
                (
                    this.ApiKey,
                    this.OrganizationName,
                    this.iOSAppName,
                    notification
                ),
                apiClient.Push
                (
                    this.ApiKey,
                    this.OrganizationName,
                    this.AndroidAppName,
                    notification
                )
            );
        }


        public async Task Send(Device device, string title, string message, IDictionary<string, string> customData)
        {
            var appName = device.IsAndroid ? this.AndroidAppName : this.iOSAppName;
            await RestService
                .For<IAppCenterApi>(this.BaseUri)
                .Push
                (
                    this.ApiKey,
                    this.OrganizationName,
                    appName,
                    new AppCenterNotification
                    {
                        Target = new NotificationTarget
                        {
                            Type = NotificationTarget.Device,
                            Devices = new[] { device.InstallId }
                        },
                        Content = new NotificationContent
                        {
                            Name = title,
                            Title = title,
                            Body = message,
                            CustomData = new Dictionary<string, string>(customData)
                        }
                    }
                );
        }

    }
}
