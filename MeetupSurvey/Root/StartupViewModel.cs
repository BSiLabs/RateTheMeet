using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Core.Device;
using MeetupSurvey.Device.Impl;
using MeetupSurvey.DTO;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using System.Reactive;
using MeetupSurvey.Infrastructure;

namespace MeetupSurvey.Root
{
    public class StartupViewModel : ViewModel
    {
        private readonly INavigationService navigationService;

        private readonly IAccountService accountService;
        private readonly IMeetupService meetupService;
        private readonly ISurveyService surveyService;
        private readonly ICoreServices core;
        private ILocalize Localize => core.Localize;

        public StartupViewModel(INavigationService navigationService, IMeetupService meetupService, ISurveyService surveyService, ICoreServices core, IAccountService accountService)
        {
            using (new PerformanceTimer("StartupViewModel.Ctor"))
            {
                this.meetupService = meetupService;
                this.navigationService = navigationService;
                this.surveyService = surveyService;
                this.core = core;

                Startup = ReactiveCommand.CreateFromTask(async () =>
                {
                    using (new PerformanceTimer("Startup"))
                    {
                        this.IsBusy = true;
                        var user = await core.Profile.GetUser();

                        if (user == null)
                        {
                            await navigationService.NavigateAsync("/NavigationPage/LoginPage");
                        }
                        else
                        {
                            try
                            {
                                //Commented this out to not refresh token on startup
                                //var token = await meetupService.RefreshToken(user);
                                //user = await core.Profile.UpdateToken(user, token);
                                //var updatedUser = await surveyService.RefreshProfile();
                                //await core.Profile.UpdateProfile(user, updatedUser);


                                await core.Profile.SignIn(user);
                                await navigationService.NavigateAsync("../MainPage/NavigationPage/SurveyListPage");
                            }
                            catch (Exception ex)
                            {
                                await core.Profile.SignOut(SignOutReason.TokenExpired);
                                await navigationService.NavigateAsync("../LoginPage");
                            }
                        }
                    }
                });

                core
                    .Network
                    .WhenStatusChanged()
                    .Where(x => !x)
                    .Subscribe(async x =>
                    {
                        await this.core.Dialogs.Alert(Localize["NetworkError"], Localize["NetworkError"], "Ok");
                    });

                networkAvailable = core
                    .Network
                    .WhenStatusChanged()
                    .ToProperty(this, x => x.NetworkAvailable);

                this.WhenAny(x => x.ShouldNavigate, y => y.NetworkAvailable,
                        (nav, network) => nav.Value == true && network.Value == true ? true : false)
                    .Subscribe((nav) =>
                        {
                            if (nav)
                            {
                                //Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                //{
                                Startup.Execute(null);
                                //});
                            }
                        }
                        , ex => { })
                    .DisposeWith(this.DestroyWith);
            }

        }
        readonly ObservableAsPropertyHelper<bool> networkAvailable;
        public bool NetworkAvailable
        {
            get { return networkAvailable.Value; }
        }

        [Reactive] public bool ShouldNavigate { get; internal set; } = false;
        public ICommand Startup { get; }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {

        }
    }
}