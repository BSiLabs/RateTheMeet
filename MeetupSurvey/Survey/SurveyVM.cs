using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Legacy;

namespace MeetupSurvey.Survey
{
    public class SurveyVM : ReactiveObject
    {
        public string Id { get; set; }
        [Reactive] public string Name { get; set; } = String.Empty;
        //[Reactive] public bool IsAdmin { get; set; }
        [Reactive] public bool HasCompleted { get; set; }
        [Reactive] public DateTimeOffset? DatePublished { get; set; }
        public bool Published => DatePublished != null ? true : false;
        public string GroupId { get; set; }

        public bool IsAdmin => DTO.Group.IsAdmin;

        [Reactive] public ReactiveList<PrizeVM> PrizeList { get; set; } = new ReactiveList<PrizeVM>() { ChangeTrackingEnabled = true };
        [Reactive] public string DatePublishedText { get; set; }
        [Reactive] public bool Archived { get; set; }
        [Reactive] public int Entries { get; set; }
        [Reactive] public double Average { get; set; }
        [Reactive] public int Submissions { get; set; }
        [Reactive] public string Status { get; set; }

        //[Reactive] public bool HasPrize { get; set; }
        [Reactive] public bool IsWinner { get; set; }

        public ICommand NavigateToSurvey { get; set; }
        public ILocalize Localize { get; set; }

        private readonly ObservableAsPropertyHelper<bool> hasPrize;
        public bool HasPrize => this.hasPrize.Value;

        private readonly ObservableAsPropertyHelper<bool> hasChanged;
        public bool HasChanged => this.hasChanged.Value;

        [Reactive] public ReactiveList<QuestionVM> QuestionList { get; set; }

        public SurveyDTO DTO { get; set; }

        public SurveyVM(SurveyDTO survey, ILocalize localize)
        {
            this.Id = survey.Id;
            this.Name = survey.Name;
            //this.IsAdmin = survey.IsAdmin;
            this.HasCompleted = survey.HasCompleted;
            this.GroupId = survey.GroupId;
            this.DatePublished = survey.DatePublished;
            this.Archived = survey.Archived;
            this.Entries = survey.Entries;
            this.Average = survey.Average;
            this.Localize = localize;
            this.QuestionList = survey.Questions != null ? 
                            survey.Questions.OrderBy(x => x.Order).Select(x => new QuestionVM(x, Localize)).ToReactiveList() 
                            : new List<QuestionVM>().ToReactiveList();

            //this.PrizeList = survey.Prizes != null ?
            //                survey.Prizes.Select(x => new PrizeVM(x, Localize)).ToReactiveList() : new List<PrizeVM>().ToReactiveList();


            hasPrize = Observable.Merge(
            PrizeList.ItemChanged
            .Select(x => PrizeList.Any()),
            PrizeList.ItemsAdded
            .Select(x => PrizeList.Any()),
            PrizeList.ItemsRemoved
            .Select(x => PrizeList.Any())
            ).ToProperty(this, x => x.HasPrize);
            if (survey.Prizes != null)
            {
                this.PrizeList.Clear();
                this.PrizeList.AddRange(survey.Prizes.Select(x => new PrizeVM(x, Localize)));
            }

            int i = 1;
            foreach(var prize in this.PrizeList)
                prize.Order = i++;

            this.DTO = survey;
            if (DatePublished != null)
            {
                DateTimeOffset datePublished = (System.DateTimeOffset)DatePublished;
                string specifier = "d";
                this.DatePublishedText = datePublished.ToString(specifier).ToString();
            }

            hasChanged = this.WhenAny(x => x.Name, (name) => (this.DTO == null ? String.Empty : this.DTO.Name) != name.Value)
                             .ToProperty(this, x => x.HasChanged);

            if (IsAdmin)
                SetAdminStatus();
            else
                SetUserStatus();
        }

        public void SetAdminStatus()
        {
            if (Published)
                Status = "Published";
            else
                Status = "Draft";
        }

        public void SetUserStatus()
        {
            if (HasCompleted)
                Status = "Completed";
            else
                Status = "Not Completed";
        }

        public SurveyDTO ToDTO()
        {
            var dto = new SurveyDTO();
            dto.Id = this.Id;
            dto.Name = this.Name;
            dto.GroupId = this.GroupId;
            dto.DatePublished = this.DatePublished;
            dto.Questions = this.QuestionList.Select(x => x.ToDTO()).ToList();
            dto.Prizes = this.PrizeList.Select(x => x.ToDTO()).ToList();
            return dto;
        }

        internal void UpdateVM(SurveyVM survey)
        {
            this.Archived = survey.Archived;
            this.Average = survey.Average;
            this.DatePublished = survey.DatePublished;
            this.DatePublishedText = survey.DatePublishedText;
            this.DTO = survey.DTO;
            this.PrizeList = survey.PrizeList;
            this.Name = survey.Name;
            this.QuestionList = survey.QuestionList;
            this.Submissions = survey.Submissions;
        }
    }
}
