using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Theming;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using MeetupSurvey.Infrastructure;

namespace MeetupSurvey.Root
{
    public class MainViewModel : ViewModel
    {
        private readonly ISurveyService surveyService;
        private readonly INavigationService _navigationService;
        private readonly ICoreServices core;
        private readonly IDeviceService device;
        private readonly IAccountService accountService;

        public ILocalize Localize => core.Localize;

        public MainViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices core,
            IDeviceService device, IAccountService accountService)
        {
            using (new PerformanceTimer("MainViewModel.Ctor"))
            {
                _navigationService = navigationService;
                this.surveyService = surveyService;
                this.core = core;
                this.device = device;
                this.accountService = accountService;
#if DEBUG
                ShowAdminToggle = true;
#endif

                CreateMenu();

                LoadCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    DTO.AuthUser user;
                    using (new PerformanceTimer("Profile.GetUser"))
                    {
                        user = await core.Profile.GetUser();
                    }

                    if (user == null)
                        await _navigationService.NavigateAsync("../LoginPage");
                    else
                    {
                        ProfileName = user.Name;
                        ProfileImage = user.Photo;
                    }
                });

                GoToWebsite = ReactiveCommand.Create(() =>
               {
                   Xamarin.Essentials.Browser.OpenAsync("https://bsilabs.ca/");
               });

                AdminToggled = ReactiveCommand.Create<bool>((isOn) =>
                {
                    surveyService.SetAdminOverride(isOn);
                });

                surveyService.GroupsRefreshed().Skip(1).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
                {
                    CreateMenu();
                }).DisposeWith(this.DestroyWith);

                ShareApp = ReactiveCommand.CreateFromTask(async () =>
                {
                    string url = "https://bsilabs.ca/ratethemeet/";
                    string title = "Share RateTheMeet";

                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Uri = url,
                        Title = title
                    });

                });
            }
        }

        public ObservableCollection<MenuItemVM> MenuItems { get; set; } = new ObservableCollection<MenuItemVM>();
        [Reactive] public MenuItemVM MenuItem { get; set; }
        [Reactive] public string SurveysCompleted { get; set; }

        [Reactive] public string ProfileImage { get; set; }
        [Reactive] public string ProfileName { get; set; }

        [Reactive] public bool IsAdmin { get; set; }
        [Reactive] public bool ShowAdminToggle { get; set; }



        public ICommand LoadCommand { get; }
        public ICommand GoToWebsite { get; }
        public ICommand ShareApp { get; }
        
        public ICommand AdminToggled { get; }

        public override void OnAppearing()
        {
            LoadCommand.Execute(null);
        }

        public void CreateMenu()
        {
            using (new PerformanceTimer("MainViewModel.CreateMenu"))
            {
                string brandFontAwesome = IconFontFamily.Get("FontAwesomeBrands", device.RuntimePlatform);
                string fontAwesome = IconFontFamily.Get("FontAwesome", device.RuntimePlatform);

                bool isAdmin = (surveyService.AdminOfAnyGroup || surveyService.OverrideAdmin);
                bool archiveVisible = MenuItems.Where(x => x.Title == "Archive").Any();
                bool shouldCreate = false;

                if ((archiveVisible && !isAdmin) || (!archiveVisible && isAdmin))
                    shouldCreate = true;

                if (!MenuItems.Any() || shouldCreate)
                {
                    MenuItems.Clear();

                    MenuItems.Add(new MenuItemVM()
                    {
                        Icon = IconFont.Home,
                        FontFamily = fontAwesome,
                        Click = ReactiveCommand.CreateFromTask(async () => { await _navigationService.NavigateAsync(nameof(NavigationPage) + "/SurveyListPage"); }),
                        Title = "Home"
                    });

                    if (surveyService.AdminOfAnyGroup || surveyService.OverrideAdmin)
                    {
                        MenuItems.Add(new MenuItemVM()
                        {
                            Icon = IconFont.Bookmark,
                            FontFamily = fontAwesome,
                            Click = ReactiveCommand.CreateFromTask(async () => { await _navigationService.NavigateAsync(nameof(NavigationPage) + "/ArchiveListPage"); }),
                            Title = "Archive"
                        });
                    }
                    MenuItems.Add(new MenuItemVM()
                    {

                        Icon = IconFont.Meetup,
                        FontFamily = brandFontAwesome,
                        Click = ReactiveCommand.CreateFromTask(async () => { await _navigationService.NavigateAsync(nameof(NavigationPage) + "/MyGroupsPage"); }),
                        Title = "My Groups"
                    });
                    MenuItems.Add(new MenuItemVM()
                    {

                        Icon = IconFont.Mobile,
                        FontFamily = fontAwesome,
                        Click = ReactiveCommand.CreateFromTask(async () => { await _navigationService.NavigateAsync(nameof(NavigationPage) + "/AboutPage"); }),
                        Title = "About"

                    });
                    MenuItems.Add(new MenuItemVM()
                    {
                        Icon = IconFont.DoorOpen,
                        FontFamily = fontAwesome,

                        Click = ReactiveCommand.CreateFromTask(async () =>
                        {
                            var logout = await core.Dialogs.Confirm(Localize["AreYouSure"], Localize["Logout"], Localize["Yes"], Localize["No"]);
                            if (logout)
                            {
                                await core.Profile.SignOut(SignOutReason.UserLoggingOut);
                                await _navigationService.NavigateAsync("/NavigationPage/LoginPage");
                                await accountService.SignOut();
                            }
                        }),
                        Title = "Log Out"

                    });
                }
            }
        }
    }
}
