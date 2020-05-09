using System;
using System.Windows.Input;
using MeetupSurvey.Core;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;

namespace MeetupSurvey.Root
{
    public class AboutViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ICoreServices _core;

        public ILocalize Localize => _core.Localize;

        public AboutViewModel(INavigationService navigationService, ICoreServices core)
        {
            _navigationService = navigationService;
            _core = core;

            Version = $"Version {VersionTracking.CurrentVersion}";

            LogIssue = ReactiveCommand.CreateFromTask(async () =>
            {
                await Browser.OpenAsync("https://github.com/BSiLabs/RateTheMeet/issues");
            });

            OpenEmail = ReactiveCommand.CreateFromTask(async() =>
            {
                var address = "appadmin@bsilabs.ca";
                await Email.ComposeAsync("RateTheMeet App", "", address);
            });
        }

        [Reactive] public string Version { get; set; }

        public ICommand LogIssue { get; }
        public ICommand OpenEmail {get;}
    }
}
