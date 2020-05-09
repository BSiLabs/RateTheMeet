 using System;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using MeetupSurvey.Core.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using MeetupSurvey.DTO;
using ReactiveUI.Legacy;
using System.Reactive.Linq;
using MeetupSurvey.Core;

namespace MeetupSurvey.Survey
{
    public class SurveyViewModel : ViewModel
    {

        readonly private ISurveyService _surveyService;
        readonly private INavigationService _navigationService;
        readonly private ICoreServices core;
        public ILocalize Localize => core.Localize;

        public SurveyViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices core)
        {
            _surveyService = surveyService;
            _navigationService = navigationService;
            this.core = core;
            LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsBusy = true;
                await LoadSurvey();
                IsBusy = false;
            });

            GoBack = ReactiveCommand.CreateFromTask(async ()=>
            {
                await _navigationService.GoBackAsync();
            });
        }

        [Reactive] public string SurveyId { get; set; }
        
        [Reactive] public SurveyDTO Survey { get; set; }
        [Reactive] public string SurveyName { get; set; }
        [Reactive] public string BackgroundImage { get; set; }

        [Reactive] public ReactiveList<QuestionVM> SurveyQuestions { get; set; }

        [Reactive] public int Position { get; set; } = 0;

        public ICommand LoadCommand { get; }
        public ICommand Submit { get; set; }
        public ICommand GoBack { get; }


        public async Task LoadSurvey()
        {
            if (SurveyId == null)
            {
                //TODO: Error message
            }
            else
            {
                Survey = await _surveyService.GetSurvey(SurveyId);
                SurveyName = Survey.Name;

                SurveyQuestions = Survey.Questions.OrderBy(x => x.Order).Select(x => new QuestionVM(x, Localize)).ToReactiveList();
                SurveyQuestions.Add(new QuestionVM(null, Localize) { IsEnd = true, CompletionText = Localize["PleaseFinish"] });

                foreach (var question in SurveyQuestions)
                {
                    question.RatingSelected = ReactiveCommand.Create<int>((parameter) =>
                    {
                        foreach (Star s in question.StarList)
                        {
                            s.IsOn = false;
                        }
                        int count = 1;
                        while (count <= parameter)
                        {
                            foreach (Star s in question.StarList)
                            {
                                if (s.Index == count)
                                {
                                    s.IsOn = !s.IsOn;
                                }
                            }
                            count++;
                        }
                        question.Rating = parameter;
                        if (question.Rating != 0)
                            question.CanSwipe = true;
                        else
                            question.CanSwipe = false;
                    });
                }

                var canExecute = SurveyQuestions.ItemChanged
                    .Where(x => x.PropertyName == "Rating")
                    .Select(y => !SurveyQuestions.Any(x => x.Rating == 0 && !x.IsEnd));

                canExecute.Subscribe(surveyCompleted =>
                {
                    if (SurveyQuestions.LastOrDefault() == null) return;
                    if (surveyCompleted)
                    {
                        SurveyQuestions.Last().CompletionText = Localize["ThanksForCompleting"];
                        Survey.HasCompleted = surveyCompleted;
                    }
                    else
                        SurveyQuestions.Last().CompletionText = Localize["PleaseFinish"];
                });

                Submit = ReactiveCommand.CreateFromTask(async () =>
                {
                    var answerList = new List<AnswerDTO>();
                    foreach (var question in SurveyQuestions)
                    {
                        var answer = new AnswerDTO()
                        {
                            QuestionId = question.Id,
                            Comment = question.Comment,
                            Rating = question.Rating
                        };
                        answerList.Add(answer);
                    }

                    await _surveyService.SubmitAnswers(answerList);
                    await GoBackAndRefresh(_navigationService,
                        new NavigationParameters
                            {{Infrastructure.KnownNavigationParameters.RemoveSurvey, Survey.Id}});

                }, canExecute
                );
            }
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if (parameters.ContainsKey("SurveyId"))
            {
                SurveyId = (string)parameters["SurveyId"];

                string backgroundImage = parameters.ContainsKey("BackgroundImage") ? (string)parameters["BackgroundImage"] : null;
                BackgroundImage = backgroundImage;

                string surveyName = parameters.ContainsKey("SurveyName") ? (string)parameters["SurveyName"] : null;
                SurveyName = surveyName;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsBusy = false;
            LoadCommand.Execute(null);
        }
    }
}
