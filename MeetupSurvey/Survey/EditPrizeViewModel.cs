using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using FFImageLoading.Forms;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using MeetupSurvey.Infrastructure;
using MeetupSurvey.Theming;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public class EditPrizeViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISurveyService _surveyService;
        private readonly ICoreServices core;
        public ILocalize Localize => core.Localize;

        public EditPrizeViewModel(INavigationService navigationService, ISurveyService surveyService, ICoreServices core)
        {
            _navigationService = navigationService;
            _surveyService = surveyService;
            this.core = core;

            LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                using (new PerformanceTimer("EditPrizeViewModel.Load"))
                {
                    var survey = await _surveyService.GetSurvey(SurveyId);
                    Survey = new SurveyVM(survey, this.Localize);

                    Observable.Merge(
                    Survey.PrizeList.ItemChanged
                    .Where(x => x.PropertyName == "HasChanged")
                    .Select(x => "ItemChanged"),
                    //Survey.PrizeList.ItemsAdded
                    //.Select(x => "CountChanged"),
                    Survey.PrizeList.ItemsRemoved
                    .Select(x => "CountChanged")
                    //,Survey.PrizeList.WhenAnyValue(x => x.Count).Select(x => "CountChanged").Skip(1)
                    ).Subscribe((val) =>
                    {//Survey.PrizeList.Any(x => x.Id == null) ||
                    if (Survey.PrizeList.Any(x => x.HasChanged) || (val == "CountChanged" && DeletedPrizes.Count > 0))
                            this.HasChanged = true;
                        else
                            this.HasChanged = false;
                    }).DisposeWith(this.DestroyWith);

                    IsLoading = false;
                }
            });
            this.RegisterBusyCommand(LoadCommand);

            AddPrize = ReactiveCommand.Create(() =>
            {
                Survey.PrizeList.Add(new PrizeVM(null, Localize) { SurveyId = SurveyId, Order = Survey.PrizeList.Count + 1 });
            });


            SavePrizes = ReactiveCommand.CreateFromTask(async () =>
            {
                using (new PerformanceTimer("EditPrizeViewModel.SavePrizes"))
                {
                    IsSaving = true;
                    //Set Prize Guids on the client so we know where to upload our photos
                    foreach (var prize in Survey.PrizeList.Where(x => x.Id == null))
                        prize.Id = Guid.NewGuid().ToString();

                    //Convert to DTO
                    var model = Survey.ToDTO();

                    //Include deleted prizes so we know what to remove on backend
                    model.Prizes.AddRange(DeletedPrizes.Select(x => x.ToDTO()));


                    //Update the Survey
                    var updatedSurvey = await _surveyService.UpdateSurvey(model);

                    //Get a task list for uploads
                    var uploadTasks = Survey.PrizeList
                                            .Where(x => x.UploadStream != null)
                                            .Select(prize => surveyService.UploadPrizePhoto(prize.Id, prize.UploadStream));
                    var results = await Task.WhenAll(uploadTasks);

                    //Temporary refresh, change UploadPrizePhoto endpoint to send back the PrizeId as well as the PhotoUrl to avoid this call
                    updatedSurvey = await _surveyService.GetSurvey(updatedSurvey.Id);

                    //Update the DTOs so we can compare changes with a base line
                    foreach (var prize in Survey.PrizeList)
                    {
                        var updatedPrize = updatedSurvey.Prizes.Where(x => x.Id == prize.Id).FirstOrDefault();

                        if (prize.DTO == null)
                        {
                            prize.DTO = updatedPrize;
                        }
                        else
                        {
                            prize.DTO.Name = updatedPrize.Name;
                            prize.DTO.Photo = updatedPrize.Photo;
                        }

                        //Reset our change tracking
                        prize.Name = null;
                        prize.Photo = null;
                        prize.Name = prize.DTO.Name;
                        prize.Photo = prize.DTO.Photo;


                        //Reset upload streams
                        prize.UploadStream = null;
                    }

                    //IsEditing = false;
                    //HasChanged = false;
                    HasSaved = true;
                    ShouldRefreshOnBack = true;
                }
            });

            ItemSelected = ReactiveCommand.Create<PrizeVM>((selected) =>
            {
                foreach (var prize in Survey.PrizeList)
                    prize.IsSelected = false;

                if(selected != null)
                    selected.IsSelected = true;
            });

            GoBack = ReactiveCommand.CreateFromTask(async () =>
            {
                bool goBack = true;
                if (this.HasChanged)
                    goBack = await this.core.Dialogs.Confirm(Localize["AreYouSure"], Localize["UnsavedChanges"], Localize["Yes"], Localize["No"]);

                if (goBack)
                {
                    if(ShouldRefreshOnBack)
                    {
                        await GoBackAndRefresh(_navigationService,
                        new NavigationParameters() { { Infrastructure.KnownNavigationParameters.UpdatePrizes, this.Survey.PrizeList } });

                    }
                    await _navigationService.GoBackAsync();
                }
            });

            PhotoOptions = ReactiveCommand.CreateFromTask(async (PrizeVM prize) =>
            {

                var actionList = new List<ActionItem>();
                if (actionList.Count == 0)
                {
                    actionList.Add(new ActionItem()
                    {
                        Icon = IconFont.Trash.ToString(),
                        Text = "Delete Photo",
                        Id = prize.Id,
                    });
                }
                var deleteRequest = await core.Dialogs.ContextAction(actionList, VerticalOptions.Center);

                if (deleteRequest != null)
                {
                    prize.Photo = null;
                    prize.UploadStream = null;
                }
            });


            DeletePrize = ReactiveCommand.CreateFromTask(async (PrizeVM prize) =>
            {
                if (prize.Id != null)
                {
                    prize.Deleted = true;
                    DeletedPrizes.Add(prize);
                }   
                Survey.PrizeList.Remove(prize);
                for (int i = 0; i < Survey.PrizeList.Count; i++)
                    Survey.PrizeList[i].Order = i + 1;
            });

            GetPhoto = ReactiveCommand.CreateFromTask(async (PrizeVM prize) =>
            {
                await CrossMedia.Current.Initialize();

                var actionList = new List<ActionItem>();
                if (actionList.Count == 0)
                {
                    if (CrossMedia.Current.IsCameraAvailable)
                    {
                        actionList.Add(new ActionItem()
                        {
                            ContextAction = ContextAction.Camera,
                            Icon = IconFont.Camera.ToString(),
                            Text = "From Camera",
                            Id = Survey.Id,
                        });
                    }
                    actionList.Add(new ActionItem()
                    {
                        ContextAction = ContextAction.Gallery,
                        Icon = IconFont.Gallery.ToString(),
                        Text = "From Gallery",
                        Id = Survey.Id,
                    });
                }
                var getPhotoAction = await core.Dialogs.ContextAction(actionList, VerticalOptions.Center);

                if (getPhotoAction != null)
                {
                    MediaFile photo = null;

                    switch (getPhotoAction.Action)
                    {
                        case ContextAction.Gallery:
                            
                            photo = await CrossMedia.Current.PickPhotoAsync();
                            break;

                        case ContextAction.Camera:
                            if (CrossMedia.Current.IsCameraAvailable)
                                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
                            break;
                    }

                    if (photo != null)
                    {
                        prize.Photo = photo.Path;

                        var stream = photo.GetStream();
                        prize.UploadStream = stream;
                    }
                }



            });

            TakePhoto = ReactiveCommand.CreateFromTask(async () =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    //DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;
            });
        }

        List<PrizeVM> DeletedPrizes { get; set; } = new List<PrizeVM>();

        [Reactive] public SurveyVM Survey { get; set; }

        [Reactive] public string SurveyId { get; set; }
        [Reactive] public string SurveyName { get; set; }
        [Reactive] public string GroupName { get; set; }
        [Reactive] public string BackgroundImage { get; set; }

        [Reactive] public bool HasChanged { get; set; }
        [Reactive] public bool IsSaving { get; set; }
        [Reactive] public bool HasSaved { get; set; }
        public bool ShouldRefreshOnBack { get; set; }
        [Reactive] public bool IsLoading { get; set; } = true;
        [Reactive] public string PageName { get; set; } = "Add Prizes";

        public ReactiveCommand<Unit,Unit> LoadCommand { get; }
        public ICommand AddPrize { get; }
        public ICommand SavePrizes { get; }
        public ICommand ItemSelected { get; }
        public ICommand GoBack { get; }
        public ICommand UploadImage { get; }
        public ICommand DeletePrize { get; }
        public ICommand GetPhoto { get; }
        public ICommand TakePhoto { get; }
        public ICommand PhotoOptions { get; }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if(parameters.TryGetValue("SurveyId", out string surveyId))
                SurveyId = surveyId;

            if(parameters.TryGetValue("GroupName", out string groupName))
                GroupName = groupName;

            await LoadCommand.Execute(Unit.Default);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            PageName = "Edit Prizes";

            parameters.TryGetValue("SurveyName", out string surveyName);
            SurveyName = surveyName;

            parameters.TryGetValue("BackgroundImage", out string backgroundImage);
            BackgroundImage = backgroundImage;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            NavigationParameters navParams = new NavigationParameters();
            navParams.Add("SurveyId", Survey.Id);
            navParams.Add("BackgroundImage", BackgroundImage);
            navParams.Add("GroupName", GroupName);
        }
    }
}
