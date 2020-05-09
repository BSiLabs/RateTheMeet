using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Globalization;
using System.Xml.Serialization;
using CarouselView.FormsPlugin.Abstractions;
using DynamicData;
using MeetupSurvey.Core.Device;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Autofac;
using MeetupSurvey.Infrastructure;
using System.Reactive.Disposables;
using System.Reactive;

namespace MeetupSurvey.Survey
{
    public enum SurveyListPageState { Default, Loading, ShowList, ShowButton, Done, HideButton }

    public class SurveyListViewModel : ViewModel
    {
        readonly private ISurveyService _surveyService;
        readonly private INavigationService _navigationService;
        readonly private ICoreServices coreServices;

        readonly private INotifications notifications;

        public ILocalize Localize => coreServices.Localize;

        public SurveyListViewModel(INavigationService navigationService, ISurveyService surveyService, IProfile profile, ICoreServices coreServices, INotifications notifications)
        {
            this.coreServices = coreServices;
            _surveyService = surveyService;
            _navigationService = navigationService;
            this.notifications = notifications;
            PullToRefreshCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await LoadCommand.Execute(true).Catch(Observable.Return(Unit.Default));
            });

            this.RequestNotificationPermission = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await this.notifications.GetStatus();

                //if (!AppCenter.Configured)
                //    AppCenter.Start(MeetupSurvey.Logging.AppCenter.Constants.AppCenterToken, typeof(Push));
                
                //else
                //    AppCenter.Start(typeof(Push));
                //AppCenter.Start(
                //    typeof(Push));
                //var enabled = await Push.IsEnabledAsync();
                //var result = await notifications.RequestPermission();


                //if (!result)
                //{
                //    result = await dialogs.Confirm(
                //        this.Localize["NotificationPermissionDeniedMessage"],
                //        this.Localize["Yes"],
                //        this.Localize["No"]
                //    );
                //    if (result)
                //        notifications.OpenSettings();
                //}
                //await navigationService.Navigate("../" + Constants.HOME_PAGE);
            });

            LoadCommand = ReactiveCommand.CreateFromTask<bool>(async (forceRefresh) =>
            {
                using (new PerformanceTimer("SurveyListViewModel.Load"))
                {
                    this.PageState = SurveyListPageState.Loading;
                    if (!InitialLoad)
                    {
                        //IsRefreshing = true;
                        this.PageState = SurveyListPageState.ShowButton;
                    }

                    AllGroups = await _surveyService.GetGroups(forceRefresh);

                    AdminOfAnyGroup = surveyService.AdminOfAnyGroup || surveyService.OverrideAdmin;

                    if (AllGroups.Count == 0)
                    {
                        HasGroups = false;

                        var group = new GroupDTO { Id = "No Groups", Name = Localize["NotAMember"], KeyPhoto = null, GroupPhoto = "Default.png", Organizers = null };

                        GroupedSurveyList.Clear();

                        GroupedSurveyList.Add(new SurveyGroup(group, true)) ;
                    }


                    else
                    {
                        var surveys = await _surveyService.GetSurveys(forceRefresh);

                        var surveyList = surveys.Select(x => new SurveyVM(x, this.Localize))
                                                .Where(x => (x.Published && ( !x.HasCompleted || x.HasPrize ))|| (x.IsAdmin && !x.Archived))
                                                .OrderByDescending(x => x.DatePublished)
                                                .ToList();

                        foreach (var survey in surveyList)
                        {
                            SurveyId = survey.Id;
                            survey.NavigateToSurvey = NavigateToSurvey;
                            if (surveyService.OverrideAdmin)
                                survey.SetAdminStatus();
                        }

                        var groups = surveyList.Select(x => x.DTO.Group)
                                               .GroupBy(x => x.Id)
                                               .Select(x => x.First())
                                               .ToList();



                        var includeGroups = AllGroups.Where(x => !groups
                                                     .Select(y => y.Id)
                                                     .Contains(x.Id) && x.IsAdmin);
                        groups.AddRange(includeGroups);

                        ////Clear whole grouped list and re-add everything
                        //GroupedSurveyList.Clear();
                        //foreach (var group in groups)
                        //{
                        //    if(surveyService.OverrideAdmin)
                        //        group.IsAdmin = true;
                            
                        //    var surveyGroup = new SurveyGroup(group, surveyList.Where(x => x.GroupId == group.Id));
                        //    GroupedSurveyList.Add(surveyGroup);
                        //}

                        //Only clear items but keep groups

                        foreach(var group in groups)
                        {
                            if (surveyService.OverrideAdmin)
                                group.IsAdmin = true;
                            var surveyGroup = new SurveyGroup(group, surveyList.Where(x => x.GroupId == group.Id));

                            var existingGroup = GroupedSurveyList.Where(x => x.GroupId == surveyGroup.GroupId).FirstOrDefault();
                            if (existingGroup != null)
                            {
                                existingGroup.Update(surveyGroup);
                                existingGroup.Clear();
                                existingGroup.AddRange(surveyGroup);
                            }
                            else
                                GroupedSurveyList.Add(surveyGroup);
                        }
                    }

                    IsRefreshing = false;

                    if (!GroupedSurveyList.Any() && AllGroups.Any())
                    {
                        var group = new GroupDTO { Id = "No Surveys", Name = Localize["NoSurveys"], KeyPhoto = null, GroupPhoto = "Default.png", Organizers = null };
                        GroupedSurveyList.Clear();
                        GroupedSurveyList.Add(new SurveyGroup(group, true));
                    }

                    if (InitialLoad)
                    {
                        //TODO: Request notifications
                        this.RequestNotificationPermission.Execute(null);

                        this.PageState = SurveyListPageState.ShowList;
                        InitialLoad = false;
                        await LoadCommand.Execute(true).Catch(Observable.Return(Unit.Default));

                    }

                    this.PageState = SurveyListPageState.ShowButton;
                    this.PageState = SurveyListPageState.Done;
                }
            });

            NavigateToSurvey = ReactiveCommand.CreateFromTask<SurveyVM>(async (survey) =>
            {
                var group = AllGroups.Where(x => x.Id == survey.GroupId).FirstOrDefault();

                PageState = SurveyListPageState.HideButton;
                await Task.Delay(200);
                NavigationParameters navParams = new NavigationParameters();
                navParams.Add("SurveyId", survey.Id);
                navParams.Add("SurveyName", survey.Name);
                

                navParams.Add("GroupName", group.Name);
                navParams.Add("BackgroundImage", group.KeyPhoto);



                if ((survey.IsAdmin || surveyService.OverrideAdmin) && !survey.Published)
                {
                    navParams.Add("Group", group);
                    await _navigationService.NavigateAsync("EditSurveyPage", navParams);
                }
                else if ((survey.IsAdmin || surveyService.OverrideAdmin) && survey.Published)
                    await _navigationService.NavigateAsync("SurveyResultPage", navParams);
                else if (survey.HasCompleted && survey.HasPrize)
                    await _navigationService.NavigateAsync("PrizePage", navParams);
                else if (!survey.HasCompleted)
                    await _navigationService.NavigateAsync("SurveyPage", navParams);
                else
                    await coreServices.Dialogs.Alert("Error", "Something went wrong");
            });

            NavigateToNewSurvey = ReactiveCommand.CreateFromTask(async () =>
            {
                PageState = SurveyListPageState.HideButton;
                await Task.Delay(200);
                var actionList = new List<ActionItem>();
                var myGroups = AllGroups.Where(x => x.IsAdmin);
                foreach (var group in myGroups)
                {
                    var groupName = group.Name.ToLower();
                    actionList.Add(new ActionItem()
                    {
                        Text = group.Name,
                        Id = group.Id
                    });
                }

                var groupId = await this.coreServices.Dialogs.ActionSheet("Select Group", actionList);

                if (groupId != null)
                {
                    var group = myGroups.Where(x => x.Id == groupId).FirstOrDefault();
                    NavigationParameters navParams = new NavigationParameters();
                    navParams.Add("GroupId", groupId);
                    navParams.Add("GroupName", group.Name);
                    navParams.Add("BackgroundImage", group.KeyPhoto);
                    navParams.Add("Group", group);
                    await _navigationService.NavigateAsync("EditSurveyPage", navParams);
                }
                else
                    PageState = SurveyListPageState.ShowButton;

            });

            _surveyService.AdminOverrided().Subscribe(async isOn =>
            {
                await LoadCommand.Execute(true).Catch(Observable.Return(Unit.Default));
            }).DisposeWith(this.DestroyWith);

            notifications.WhenNewSurvey().Subscribe(async _ =>
            {
                await LoadCommand.Execute(true).Catch(Observable.Return(Unit.Default));
            }).DisposeWith(this.DestroyWith);
        }

        [Reactive] public bool HasGroups { get; set; } = true;
        [Reactive] public bool AdminOfAnyGroup { get; set; }
        [Reactive] public bool CanEdit { get; set; } = true;

        [Reactive] public bool IsRefreshing { get; set; }
        public ObservableCollection<SurveyGroup> GroupedSurveyList { get; set; } = new ObservableCollection<SurveyGroup>();
        public List<GroupDTO> AllGroups { get; set; }
        [Reactive] public bool InitialLoad { get; set; } = true;
        [Reactive] public SurveyListPageState PageState { get; set; }

        [Reactive] public string SurveyId { get; set; }
        [Reactive] public SurveyResultDTO SurveyResult { get; set; }
        public ObservableCollection<QuestionResultVM> SurveyQuestionResults { get; set; } = new ObservableCollection<QuestionResultVM>();

        public ICommand PullToRefreshCommand { get; set; }
        public ReactiveCommand<bool, System.Reactive.Unit> LoadCommand { get; }
        public ICommand NavigateToSurvey { get; }
        public ICommand NavigateToNewSurvey { get; }
        public ICommand RequestNotificationPermission { get; }
        public ICommand GoToWebsite { get; }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            //If we should remove a survey
            if (parameters.TryGetValue(Infrastructure.KnownNavigationParameters.RemoveSurvey, out string surveyId))
            {
                if (GroupedSurveyList != null)
                {
                    var surveyToRemove =
                        GroupedSurveyList.SelectMany(surveyGroup => surveyGroup.Where(survey => survey.Id == surveyId)).FirstOrDefault();

                    if (surveyToRemove != null)
                    {

                        foreach (var surveyGroup in GroupedSurveyList)
                        {
                            if (!surveyGroup.Contains(surveyToRemove)) continue;
                            surveyGroup.Remove(surveyToRemove);
                            break;
                        }
                    }
                }
            }
            else if (parameters.TryGetValue(Infrastructure.KnownNavigationParameters.AddSurvey, out SurveyVM survey))
            {
                var group = GroupedSurveyList.Where(x => x.GroupId == survey.GroupId).FirstOrDefault();
                if (group != null)
                {
                    var existingSurvey = group.ToList().Where(x => x.Id == survey.Id).FirstOrDefault();
                    if (existingSurvey == null)
                        group.Add(survey);
                    else
                    {
                        existingSurvey.UpdateVM(survey);
                    }
                }
            }
            else if (parameters.TryGetValue(Infrastructure.KnownNavigationParameters.UpdateSurvey, out SurveyVM updatedSurvey))
            {
                var group = GroupedSurveyList.Where(x => x.GroupId == updatedSurvey.GroupId).FirstOrDefault();
                if (group != null)
                {
                    var existingSurvey = group.ToList().Where(x => x.Id == updatedSurvey.Id).FirstOrDefault();
                    if (existingSurvey != null)
                    { 
                        existingSurvey.UpdateVM(updatedSurvey);
                    }
                }
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.Back && parameters.TryGetValue("ShouldRefresh", out bool shouldRefresh))
                await LoadCommand.Execute(true).Catch(Observable.Return(Unit.Default));
            else if (parameters.GetNavigationMode() == NavigationMode.New)
                await LoadCommand.Execute(false).Catch(Observable.Return(Unit.Default));
            else
            {
                PageState = SurveyListPageState.ShowButton;
                PageState = SurveyListPageState.Done;
            }
        }
    }
}
