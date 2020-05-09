using System;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using Prism.Navigation;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace MeetupSurvey.Account
{
    public class LoginViewModel : ViewModel
    {
        IDisposable signInSubject;
        readonly private ICoreServices core;
        public ILocalize Localize => core.Localize;


        private readonly IMeetupService meetupService;
        private readonly IProfile profile;

        public LoginViewModel(INavigationService navigationService, IAccountService accountService, IMeetupService meetupSevice, IProfile profile, ICoreServices core)
        {
            this.meetupService = meetupSevice;
            this.profile = profile;
            this.core = core;

            profile.WhenProfileError().Subscribe(async (error) =>
            {
                await accountService.SignOut();
                await navigationService.NavigateAsync("/NavigationPage/LoginPage");
                await core.Dialogs.Alert("Error", error);
            }
            , ex => { })
            .DisposeWith(this.DestroyWith);

            Login = ReactiveCommand.Create(() =>
            {
                accountService.Authenticate();
            });

            this.signInSubject = this.profile
                .WhenUserSignedIn()
                .Subscribe(async _ =>
                {
                    await navigationService.NavigateAsync("/MainPage/NavigationPage/SurveyListPage");
                }
                , ex => { })
                .DisposeWith(this.DestroyWith);

        }

        public ICommand Login { get; }
        public ICommand Signup { get; }
    }
}