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

namespace MeetupSurvey.Survey
{
    public enum ArchiveListPageState { Default, Loading, ShowList, Done }

    public class ArchiveListViewModel : ViewModel
    { 
        private readonly ISurveyService _surveyService;
        private readonly INavigationService _navigationService;
        readonly private ICoreServices coreServices;

        public ILocalize Localize => coreServices.Localize;

        public ArchiveListViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices core)
        {
            _surveyService = surveyService;
            _navigationService = navigationService;
            coreServices = core;

            LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsRefreshing = true;
                this.PageState = ArchiveListPageState.Loading;

                var user = await coreServices.Profile.GetUser();



                var surveys = await _surveyService.GetArchivedSurveys();
                var surveyList = surveys.Select(x => new SurveyVM(x, this.Localize)).ToList();

                foreach (var survey in surveyList)
                {
                    SurveyId = survey.Id;
                    survey.NavigateToSurvey = NavigateToSurvey;
                }


                var groups = surveys.Select(x => x.Group)
                                    .GroupBy(x => x.Id)
                                    .Select(x => x.First())
                                    .ToList();

                AllGroups = await _surveyService.GetGroups(false);



                var includeGroups = AllGroups.Where(x => !groups.Select(y => y.Id).Contains(x.Id) && x.IsAdmin);
                groups.AddRange(includeGroups);

                GroupedSurveyList.Clear();
                foreach (var group in groups)
                {
                    var surveyGroup = new SurveyGroup(group, surveyList.Where(x => x.GroupId == group.Id));
                    GroupedSurveyList.Add(surveyGroup);
                }

                IsRefreshing = false;

                if (InitialLoad)
                {
                    await Task.Delay(250);
                    this.PageState = ArchiveListPageState.ShowList;
                    InitialLoad = false;
                }

                this.PageState = ArchiveListPageState.Done;
            });

            NavigateToSurvey = ReactiveCommand.CreateFromTask<SurveyVM>(async (survey) =>
            {
                NavigationParameters navParams = new NavigationParameters();
                navParams.Add("SurveyId", survey.Id);

                if (survey.IsAdmin)
                    await _navigationService.NavigateAsync("SurveyResultPage", navParams);
                else
                    await _navigationService.NavigateAsync("SurveyResultPage", navParams);
            });

        }

        [Reactive] public bool IsRefreshing { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public List<SurveyVM> Surveys { get; set; }

        [Reactive] public string SurveyId { get; set; }
        [Reactive] public SurveyResultDTO SurveyResult { get; set; }
        public ObservableCollection<QuestionResultVM> SurveyQuestionResults { get; set; } = new ObservableCollection<QuestionResultVM>();

        public ObservableCollection<SurveyGroup> GroupedSurveyList { get; set; } = new ObservableCollection<SurveyGroup>();
        [Reactive] public List<GroupVM> Groups { get; set; }

        [Reactive] public ArchiveListPageState PageState { get; set; }
        [Reactive] public bool InitialLoad { get; set; } = false;

        public List<GroupDTO> AllGroups { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand NavigateToSurvey { get; }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            bool shouldRefresh = true;
            if (parameters.GetNavigationMode() == NavigationMode.Back)
            {
                parameters.TryGetValue(Infrastructure.KnownNavigationParameters.ShouldRefresh, out shouldRefresh);
            }
            if (shouldRefresh)
                LoadCommand.Execute(null);
        }

        public async Task LoadSurveys()
        {
            var surveys = await _surveyService.GetArchivedSurveys();
            Surveys = surveys.Select(x => new SurveyVM(x, this.Localize)).ToList();
        }
    }
}
