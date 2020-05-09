using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Core.Device;
using MeetupSurvey.DTO;
using MeetupSurvey.Infrastructure;
using MeetupSurvey.Theming;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Legacy;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public class PrizeViewModel : ViewModel
    {
        readonly IDisposable prizeResultSub;

        public enum PrizePageState { Default, Loading, Done, ShowWinner, ShakeBox }
        private readonly INavigationService _navigationService;
        private readonly ISurveyService _surveyService;
        private readonly ICoreServices coreServices;
        private readonly INotifications notificationService;

        public ILocalize Localize => coreServices.Localize;

        public PrizeViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices core, INotifications notifications)
        {
            _navigationService = navigationService;
            _surveyService = surveyService;
            this.coreServices = core;
            this.notificationService = notifications;

            

            LoadCommand = ReactiveCommand.CreateFromTask<bool>(async suppressAnimation =>
            {
                using (new PerformanceTimer("PrizeViewModel.Load"))
                {
                    var user = await core.Profile.GetUser();

                    if (SurveyId == null)
                    {
                        //TODO: Error message
                    }
                    else
                    {
                        var survey = await _surveyService.GetSurvey(SurveyId);
                        Survey = new SurveyVM(survey, Localize);

                        if (!suppressAnimation)
                            PageState = PrizePageState.Loading;

                        var prizeResultDTO = await _surveyService.GetPrizeResult(SurveyId);

                        SurveyName = prizeResultDTO.SurveyName;

                        Prizes.Clear();
                        Prizes.AddRange(prizeResultDTO.Prizes.Select(x => new PrizeVM(x, this.Localize)));


                        foreach (var prize in Prizes)
                        {
                            if (!prize.HasWinner && Survey.IsAdmin)
                                prize.CanExecutePrizeDraw = true;
                            else
                                prize.CanExecutePrizeDraw = false;
                        }

                        Entries = prizeResultDTO.EntryCount;
                        Image = prizeResultDTO.Image;
                    }

                    PageState = PrizePageState.ShowWinner;
                    PageState = PrizePageState.Done;
                }
            });

            GoBack = ReactiveCommand.CreateFromTask(async () =>
            {
                await _navigationService.GoBackAsync();
            });

            StartPrizeDraw = ReactiveCommand.CreateFromTask<PrizeVM>(async (prize) =>
            {
                if (!prize.ShowedWinner && prize.WinnerName == null)
                {
                    prize.CanExecutePrizeDraw = false;
                    var winner = await surveyService.StartPrizeDraw(prize.Id);
                    prize.WinnerUserAccountId = winner.UserAccountId;
                    prize.WinnerName = winner.Name;
                    prize.WinnerPhoto = winner.Photo;
                    PageState = PrizePageState.ShakeBox;
                    PageState = PrizePageState.Done;
                }
            });

            ShowBanner = ReactiveCommand.Create(() =>
            {
                PageState = PrizePageState.ShowWinner;
                PageState = PrizePageState.Done;
            });

            More = ReactiveCommand.CreateFromTask(async () =>
            {

                var actionList = new List<ActionItem>();
                if (!actionList.Any())
                {
                    actionList.Add(new ActionItem()
                    {
                        Icon = IconFont.Edit.ToString(),
                        Text = "Edit Prizes",
                        Id = Survey.Id,
                        ContextAction = ContextAction.Edit
                    });
                }

                var contextResult = await core.Dialogs.ContextAction(actionList, VerticalOptions.Start);

                if (contextResult != null)
                {
                    switch (contextResult.Action)
                    {
                        case ContextAction.Edit:
                            NavigationParameters navParams = new NavigationParameters();
                            navParams.Add("SurveyId", SurveyId);
                            await _navigationService.NavigateAsync("EditPrizePage", navParams);
                            break;
                    }

                }
            });


            this.prizeResultSub = this.notificationService.WhenPrizeNotified().Subscribe<Notification>(notification =>
            {
                PrizeNotified(notification);
            }, ex =>
            {

            }).DisposeWith(this.DestroyWith);

            
        }

        public void PrizeNotified(Notification notification)
        {
            var data = notification.CustomData;

            var prizeId = data["PrizeId"];
            var winnerId = data["WinnerId"];
            var winnerName = data["WinnerName"];
            var winnerPhoto = data["WinnerPhoto"];

            var prize = Prizes.Where(x => x.Id == prizeId).FirstOrDefault();
            var index = Prizes.IndexOf(prize);

            //SelectedPrize = prize;
            prize.CanExecutePrizeDraw = false;
            prize.WinnerName = winnerName;
            prize.WinnerPhoto = winnerPhoto;
            prize.WinnerUserAccountId = winnerId;

            Position = Prizes.IndexOf(prize);

            PageState = PrizePageState.ShakeBox;
            PageState = PrizePageState.Done;
        }

        [Reactive] public int Position { get; set; } = 0;
        [Reactive] public SurveyVM Survey { get; set; }
        [Reactive] public PrizeVM SelectedPrize { get; set; }
        [Reactive] public string SurveyId { get; set; }

        [Reactive] public bool CanEditPrizes { get; set; }

        public ObservableCollection<PrizeVM> Prizes { get; set; } = new ObservableCollection<PrizeVM>();


        // prize image
        [Reactive] public string Image { get; set; }
        [Reactive] public int Entries { get; set; }
        [Reactive] public string SurveyName { get; set; }

        [Reactive] public bool WinnersSelected { get; set; } = true;

        [Reactive] public PrizePageState PageState { get; private set; }

        public ICommand LoadCommand { get; }
        public ICommand GoBack { get; }
        public ICommand StartPrizeDraw { get; }
        public ICommand ShowBanner { get; }
        public ICommand More { get; }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SurveyId"))
            {
                SurveyId = (string)parameters["SurveyId"];

                LoadCommand.Execute(false);
            }
            else if (parameters.GetNavigationMode() == NavigationMode.Back && parameters.TryGetValue(Infrastructure.KnownNavigationParameters.UpdatePrizes, out ReactiveList<PrizeVM> updatedPrizes))
            {
                //Prizes.Clear();
                //Prizes.AddRange(updatedPrizes);
                //PageState = PrizePageState.ShowWinner;
                //PageState = PrizePageState.Done;
                List<PrizeVM> deleteList = new List<PrizeVM>();
                foreach(var prize in Prizes)
                {
                    var updatedPrize = updatedPrizes.Where(x => x.Id == prize.Id).FirstOrDefault();
                    if (updatedPrize == null)
                        deleteList.Add(prize);
                    else
                    {
                        prize.Name = updatedPrize.Name;
                        prize.Photo = updatedPrize.Photo;
                    }
                }

                foreach (var prize in updatedPrizes)
                {
                    var existingPrize = Prizes.Where(x => x.Id == prize.Id).FirstOrDefault();
                    if (existingPrize == null)
                        Prizes.Add(prize);
                }

                foreach(var prize in deleteList)
                {
                    Prizes.Remove(prize);
                }
            }
        }
    }
}
