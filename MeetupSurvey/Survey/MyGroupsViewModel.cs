using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using MeetupSurvey.Infrastructure;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public enum MyGroupsListPageState { Default, Loading, ShowList, Done }
    public class MyGroupsViewModel : ViewModel
    {
        private readonly ISurveyService _surveyService;
        private readonly INavigationService _navigationService;
        private readonly ICoreServices coreServices;
        public ILocalize Localize => coreServices.Localize;

        public MyGroupsViewModel(INavigationService navigationService, ISurveyService surveyService, IProfile profile, ICoreServices coreServices)
        {
            _surveyService = surveyService;
            _navigationService = navigationService;
            this.coreServices = coreServices;

            LoadGroups = ReactiveCommand.CreateFromTask(async () =>
            {
                using (new PerformanceTimer("MyGroupsViewModel.LoadGroups"))
                {
                    this.PageState = MyGroupsListPageState.Loading;

                    var user = await profile.GetUser();

                    // var surveys = await _surveyService.GetSurveys();

                    var allGroups = await _surveyService.GetGroups(false);

                    GroupList.Clear();
                    foreach (var group in allGroups)
                    {
                        Console.WriteLine(group.Name);
                        var surveyGroup = new SurveyGroup(group, null);
                        GroupList.Add(surveyGroup);
                    }

                    if (GroupList.Count == 0)
                    {
                        HasGroups = false;

                        var group = new GroupDTO { Id = null, Name = Localize["NotAMember"], KeyPhoto = null, GroupPhoto = null, Organizers = null };

                        GroupList.Add(new SurveyGroup(group));
                    }

                    this.PageState = MyGroupsListPageState.ShowList;

                    IsRefreshing = false;
                    this.PageState = MyGroupsListPageState.Done;
                }
            });
        }

        [Reactive] public bool HasGroups { get; set; } = true;
        public ObservableCollection<SurveyGroup> GroupList { get; set; } = new ObservableCollection<SurveyGroup>();
        [Reactive] public bool IsRefreshing { get; set; }
        public ICommand LoadGroups { get; }
        
        [Reactive] public MyGroupsListPageState PageState { get; set; }


        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            LoadGroups.Execute(null);
        }
    }
}
