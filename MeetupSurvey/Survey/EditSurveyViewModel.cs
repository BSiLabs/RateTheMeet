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
using Prism.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using ReactiveUI.Legacy;
using MeetupSurvey.Theming;
using System.Reactive.Disposables;
using System.Reactive;

namespace MeetupSurvey.Survey
{
    public class EditSurveyViewModel : ViewModel
    {
        readonly private IProfile _profile;
        readonly private ISurveyService surveyService;
        readonly private INavigationService _navigationService;
        readonly private ICoreServices core;
        public ILocalize Localize => core.Localize;



        public EditSurveyViewModel(ISurveyService surveyService, IProfile profile, INavigationService navigationService, ICoreServices core)
        {
            _profile = profile;
            this.surveyService = surveyService;
            _navigationService = navigationService;
            this.core = core;

            PageName = this.Localize["NewSurvey"];
            PrizeButtonText = this.Localize["AddPrizes"];

            Load = ReactiveCommand.CreateFromTask(async () =>
            {
                IsLoading = true;

                if (SurveyId != null)
                {
                    IsEditing = false;
                    var survey = await this.surveyService.GetSurvey(SurveyId);
                    this.Survey = new SurveyVM(survey, this.Localize);

                    if (!Survey.PrizeList.IsEmpty)
                        PrizeButtonText = "EDIT PRIZES";

                    IsEditing = false;
                    HasChanged = false;
                    CanAddPrizes = true;
                }
                else //IsNew
                {
                    IsEditing = true;
                    this.Survey = new SurveyVM(new SurveyDTO() { Group = this.Group, GroupId = this.Group.Id }, this.Localize);
                }


                Observable.Merge(
                Survey.QuestionList.ItemChanged
                    .Where(x => x.PropertyName == "HasChanged")
                    .Select(x => "ItemChanged"),
                    Survey.WhenAnyValue(x => x.Name)
                    .Select(x => "NameChanged"),
                    Survey.QuestionList.WhenAnyValue(x => x.Count).Select(x => "CountChanged").Skip(1)
                    ).Subscribe((val) =>
                    {
                        if (Survey.QuestionList.Any(x => x.HasChanged) || (Survey.DTO == null ? String.Empty : Survey.DTO.Name) != Survey.Name || val == "CountChanged")
                            this.HasChanged = true;
                        else
                            this.HasChanged = false;
                    }).DisposeWith(this.DestroyWith);
                IsLoading = false;
            });
            

            SaveSurvey = ReactiveCommand.CreateFromTask(async () =>
            {
                if(String.IsNullOrEmpty(Survey.Name))
                {
                    Survey.Name = "New Survey - " + DateTime.Now.ToString("MMM dd, yyyy");
                }
                var model = Survey.ToDTO();

                if (model.Id == null)
                {
                    var updatedSurvey = await surveyService.AddSurvey(model);
                    this.Survey = new SurveyVM(updatedSurvey, this.Localize);
                    Console.WriteLine(updatedSurvey.Id);
                    addedSurvey = true;
                }
                else
                {
                    //Add back the deleted questions to the list so we know what to delete on backend
                    model.Questions.AddRange(DeletedQuestions.Select(x => x.ToDTO()));
                    await surveyService.UpdateSurvey(model);
                }
                    
                IsEditing = false;
                HasChanged = false;
                HasSaved = true;
                CanAddPrizes = true;
            });

            GoBack = ReactiveCommand.CreateFromTask(async () =>
            {
                bool goBack = true;
                if(this.HasChanged)
                    goBack = await core.Dialogs.Confirm(Localize["AreYouSure"], Localize["UnsavedChanges"], Localize["Yes"], Localize["No"]);

                if (goBack)
                {
                    if(HasSaved)
                    {
                        await this.GoBackAndRefresh(navigationService);
                    }
                    else
                        await _navigationService.GoBackAsync();
                }
            });

            AddQuestion = ReactiveCommand.Create(() =>
            {
                Survey.QuestionList.Add(new QuestionVM(null, Localize) { Order = Survey.QuestionList.Count + 1 , SurveyId = this.Survey.Id});
            });

            ItemSelected = ReactiveCommand.Create<QuestionVM>((selected) =>
            {
                if(IsEditing)
                {
                    foreach (var question in Survey.QuestionList)
                        question.IsSelected = false;

                    if(selected != null)
                        selected.IsSelected = true;
                }
                else
                {
                    //TODO: Show toast that instructs user to tap edit button
                    HintEditButton = true;
                }
            });

            DeleteSurvey = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Survey.Id == null)
                    await GoBackAndRefresh(_navigationService, null);
                else
                {
                    try
                    {
                        await surveyService.DeleteSurvey(Survey.Id);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                        core.Log.WriteEvent($"DeleteSurveyException: {Survey.Id}");
                        ShouldNavigate = false;
                        await this.core.Dialogs.Alert(Localize["Error"], Localize["ErrorDeletingSurvey"], Localize["Ok"]);
                    }

                    if (ShouldNavigate)
                        await GoBackAndRefresh(_navigationService,
                            new NavigationParameters() { { Infrastructure.KnownNavigationParameters.RemoveSurvey, Survey.Id } });
                }
            });

            EditSurvey = ReactiveCommand.Create( () =>
            {
                IsEditing = true;
            });

            Publish = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Survey.Id != null)
                {
                    if (Survey.QuestionList.Count > 0)
                    {
                        var notify = await this.core.Dialogs.Confirm("Notify Group Members", "Do you want to notify members of your group?", "Yes", "No");
                        
                        await surveyService.PublishSurvey(new PublishSurveyArgs(Survey.Id, false, notify));
                
                        await this.GoBackAndRefresh(_navigationService, new NavigationParameters() { { Infrastructure.KnownNavigationParameters.UpdateSurvey, Survey } });
                    }
                    else
                    {
                        await this.core.Dialogs.Alert("No Questions", Localize["OneQuestion"], Localize["Ok"]);
                    }
                }
            });

            More = ReactiveCommand.CreateFromTask(async () =>
            {

                var actionList = new List<ActionItem>();
                if (!actionList.Any())
                {
                    actionList.Add(new ActionItem()
                    {
                        Icon = IconFont.Trash.ToString(),
                        Text = "Delete",
                        Id = Survey.Id,
                    });
                }

                var contextResult = await core.Dialogs.ContextAction(actionList, VerticalOptions.Start);

                if (contextResult != null)
                {
                    switch(contextResult.Action)
                    {
                        case ContextAction.Delete:
                            DeleteSurvey.Execute(null);
                            break;
                    }

                }
            });

            DeleteQuestion = ReactiveCommand.Create((QuestionVM question) =>
            {
                if (question.Id != null)
                {
                    question.Deleted = true;
                    DeletedQuestions.Add(question);
                }
                Survey.QuestionList.Remove(question);
                for(int i = 0; i < Survey.QuestionList.Count; i++)
                    Survey.QuestionList[i].Order = i+1;
            });
           
            NavigateToEditPrize = ReactiveCommand.CreateFromTask(async () =>
            {
                bool canNav = true;
                if(!HasSaved && HasChanged)
                {
                    var save = await core.Dialogs.Confirm(Localize["Save"], Localize["SaveSurvey"], Localize["Save"], Localize["Cancel"]);
                    if (save)
                        await SaveSurvey.Execute();
                    else
                        canNav = false;
                }

                if(canNav)
                {
                    NavigationParameters navParams = new NavigationParameters();
                    navParams.Add("SurveyId", Survey.Id);
                    navParams.Add("SurveyName", Survey.Name);
                    navParams.Add("GroupName", GroupName);
                    navParams.Add("BackgroundImage", BackgroundImage);
                    await _navigationService.NavigateAsync("EditPrizePage", navParams);
                }
            });

            PreviousQuestionPopup = ReactiveCommand.CreateFromTask<QuestionVM>(async (questionVM) =>
            {
                if(PreviousQuestions == null)
                    PreviousQuestions = await surveyService.GetPreviousQuestions();

                var previousQuestion = await this.core.Dialogs.ActionSheet("Previous Questions", PreviousQuestions.Select(x => new ActionItem() { Id = x.Name, Text = x.Name }).ToList());
                if (previousQuestion != null)
                    questionVM.Name = previousQuestion;
            });
        }

        //Nav Props
        [Reactive] public string SurveyId { get; set; }
        //[Reactive] public string GroupId { get; set; }
        [Reactive] public string GroupName { get; set; }
        public GroupDTO Group { get; set; }


        List<QuestionDTO> PreviousQuestions { get; set; }
        List<QuestionVM> DeletedQuestions { get; set; } = new List<QuestionVM>();

        [Reactive] public bool ShouldNavigate { get; set; } = true;
        [Reactive] public bool HasPrize { get; set; }
        [Reactive] public bool IsEditing { get; set; }
        [Reactive] public bool CanAddPrizes { get; set; }
        [Reactive] public SurveyVM Survey { get; set; }
        [Reactive] public string PageName { get; set; } = "New Survey";
        [Reactive] public bool HasChanged { get; set; }
        [Reactive] public bool HasSaved { get; set; }
        [Reactive] public string BackgroundImage { get; set; }
        [Reactive] public string PrizeButtonText { get; set; } = "ADD PRIZES";
        [Reactive] public bool IsLoading { get; set; } = true;
        [Reactive] public bool HintEditButton { get; set; }

        public ReactiveCommand<Unit, Unit> SaveSurvey { get; } 
        public ICommand DeleteSurvey { get; }
        public ICommand AddQuestion { get; }
        public ICommand ItemSelected { get; set; }
        public ICommand GoBack { get; set; }
        public ICommand EditSurvey { get; set; }
        public ICommand Publish { get; set; }
        public ICommand PreviousQuestionPopup { get; set; }

        public ICommand Load { get; set; }
        public ICommand More { get; set; }
        
        public ICommand DeleteQuestion { get; }
        public ICommand NavigateToEditPrize { get; }

        private bool addedSurvey = false;

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.GetNavigationMode() != NavigationMode.Back)
            {
                parameters.TryGetValue("SurveyId", out string surveyId);
                parameters.TryGetValue("Group", out GroupDTO group);
                //parameters.TryGetValue("GroupName", out string groupName);
                //parameters.TryGetValue("GroupId", out string groupId);

                SurveyId = surveyId;

                GroupName = group.Name;
                //GroupId = group.Id;
                Group = group;

                //GroupName = groupName;
                //GroupId = groupId;

                Load.Execute(null);
            }
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.GetNavigationMode() != NavigationMode.Back)
            {
                parameters.TryGetValue("BackgroundImage", out string backgroundImage);
                BackgroundImage = backgroundImage;
            }

            bool isExisting = parameters.TryGetValue("SurveyId", out string surveyId);

            if (isExisting || Survey?.Id != null)
            {
                IsEditing = false;
                PageName = "Edit Survey";
            }
            else
            {
                IsEditing = true;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if(addedSurvey)
                parameters.Add(Infrastructure.KnownNavigationParameters.AddSurvey, this.Survey);
            else if (HasSaved)
                parameters.Add(Infrastructure.KnownNavigationParameters.UpdateSurvey, this.Survey);
            base.OnNavigatedFrom(parameters);

        }
    }
}
