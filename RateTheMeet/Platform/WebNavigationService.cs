using MeetupSurvey.Core.Device;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RateTheMeet.Platform
{
    public class WebNavigationService : IWebNavigationService
    {
        private readonly NavigationManager _navigationManager;

        public WebNavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }
        public Task ComposeEmailAsync(string subject, string body, params string[] to)
        {
            throw new NotImplementedException();
        }

        public Task NavigateAsync(string url)
        {
            _navigationManager.NavigateTo(url);
            return Task.CompletedTask;
        }
    }
}
