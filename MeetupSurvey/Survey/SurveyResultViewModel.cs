using MeetupSurvey.Core.Data;
using Prism.Navigation;
using System.Windows.Input;
using Microcharts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using MeetupSurvey.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using MeetupSurvey.Core;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MeetupSurvey.Dialogs;
using MeetupSurvey.Theming;
using DynamicData;
using MeetupSurvey.Infrastructure;

namespace MeetupSurvey.Survey
{
    public enum SurveyResultPageState { Default, Loading, ShowScreen, Done }
    public class SurveyResultViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISurveyService _surveyService;
        private readonly ICoreServices _coreServices;
        public ILocalize Localize => _coreServices.Localize;

        public SurveyResultViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices coreServices)
        {
            _navigationService = navigationService;
            _surveyService = surveyService;
            _coreServices = coreServices;


            LoadResults = ReactiveCommand.CreateFromTask(async () =>
            {
                using (new PerformanceTimer("SurveyResultViewModel.LoadResults"))
                {
                    if (SurveyId == null)
                    {
						//TODO: Error message
						this.GoBack.Execute(null);
					}
                    else
                    {
                        PageState = SurveyResultPageState.Loading;
                        try
                        {
                            SurveyResult = await surveyService.GetSurveyResult(SurveyId);
                            if (SurveyResult == null)
                                this.GoBack.Execute(null);
                            else
                            {
                                SurveyName = SurveyResult.SurveyName;

                                var survey = await surveyService.GetSurvey(SurveyId);
                                HasPrize = survey.Prizes.Any();

                                SurveyQuestionResults.Clear();
                                SurveyQuestionResults.AddRange(SurveyResult.QuestionResults
                                                                    .Select(x => new QuestionResultVM(x)));

                                if (SurveyQuestionResults.Any())
                                    HasData = true;
                                else
                                    HasData = false;

                                PageState = SurveyResultPageState.ShowScreen;
                                PageState = SurveyResultPageState.Done;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            this.GoBack.Execute(null);
                        }
                    }
                }
            });

            GoBack = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync();
            });

            GoToPrize = ReactiveCommand.CreateFromTask(async () =>
            {
                NavigationParameters navParams = new NavigationParameters();
                navParams.Add("SurveyId", this.SurveyId);
                
                await _navigationService.NavigateAsync("PrizePage", navParams);
            });

            More = ReactiveCommand.CreateFromTask(async () =>
            {
                var actionList = new List<ActionItem>();
                if (actionList.Count == 0)
                {
                    actionList.Add(new ActionItem()
                    {
                        Text = "View Prizes",
                        Id = SurveyId,
                        Icon = IconFont.Award,
                        ContextAction = ContextAction.ViewPrizes
                    });

                    if (!SurveyResult.Archived)
                    {
                        actionList.Add(new ActionItem()
                        {
                            Text = "Un-publish",
                            Id = SurveyId,
                            Icon = IconFont.Rocket,
                            ContextAction = ContextAction.Unpublish
                        });

                        actionList.Add(new ActionItem()
                        {
                            Text = "Archive",
                            Id = SurveyId,
                            Icon = IconFont.Bookmark,
                            ContextAction = ContextAction.Archive
                        });
                    }
                    actionList.Add(new ActionItem()
                    {
                        Text = "Delete",
                        Id = SurveyId,
                        Icon = IconFont.Trash,
                        ContextAction = ContextAction.Delete
                    });
                }

                var request = await _coreServices.Dialogs.ContextAction(actionList, VerticalOptions.Start);
              
                if (request != null)
                {
                    var action = (request as ContextMenuAction);
                    switch(action.Action)
                    {
                        case ContextAction.Delete:
                            var delete = await coreServices.Dialogs.Confirm(Localize["AreYouSure"], Localize["SureDelete"], Localize["Yes"], Localize["No"]);
                            if(delete)
                            {
                                await _surveyService.DeleteSurvey(action.Id);
                                await this.GoBackAndRefresh(_navigationService);
                            }
                            break;
                        case ContextAction.Unpublish:
                            var unpublish = await coreServices.Dialogs.Confirm(Localize["AreYouSure"], Localize["SureUnpublish"], Localize["Yes"], Localize["No"]);
                            if (unpublish)
                            {
                                await _surveyService.PublishSurvey(new PublishSurveyArgs(action.Id, true, false));
                                await this.GoBackAndRefresh(_navigationService);
                            }
                            break;
                        case ContextAction.Archive:
                            var archive = await coreServices.Dialogs.Confirm(Localize["AreYouSure"], Localize["SureArchive"], Localize["Yes"], Localize["No"]);
                            if (archive)
                            {
                                await surveyService.ArchiveSurvey(action.Id);
                                await this.GoBackAndRefresh(_navigationService);
                            }
                            break;
                        case ContextAction.ViewPrizes:
                            NavigationParameters navParams = new NavigationParameters();
                            navParams.Add("SurveyId", this.SurveyId);
                            await _navigationService.NavigateAsync("PrizePage", navParams);
                            break;
                    }
                }
            });
        }


        [Reactive] public int Position { get; set; } = 0;
        [Reactive] public bool HasData { get; set; } = true;

        [Reactive] public string SurveyName { get; set; }
        [Reactive] public string SurveyId { get; set; }
        [Reactive] public bool HasPrize { get; set; }

        [Reactive] public SurveyResultDTO SurveyResult { get; set; }
        public ObservableCollection<QuestionResultVM> SurveyQuestionResults { get; set; } = new ObservableCollection<QuestionResultVM>();
     
        [Reactive] public SurveyResultPageState PageState { get; set; }

        public ICommand LoadResults { get; set; }
        public ICommand GoBack { get; set; }
        public ICommand More { get; set; }
        public ICommand GoToPrize { get; set; }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SurveyId"))
            {
                SurveyId = (string)parameters["SurveyId"];
                LoadResults.Execute(null);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            this.PageState = SurveyResultPageState.Default;
        }

    }
}
