using System;
using System.IO;
using MeetupSurvey.Core;
using MeetupSurvey.DTO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public class PrizeVM : ReactiveObject
    {
        [Reactive] public string Id { get; set; }
        [Reactive] public string Name { get; set; } = string.Empty;
        [Reactive] public string Photo { get; set; }
        [Reactive] public string SurveyId { get; set; }
        [Reactive] public bool HasLoaded { get; set; }
        [Reactive] public bool IsSelected { get; set; }

        [Reactive] public int Order { get; set; }

        [Reactive] public string WinnerName { get; set; }
        [Reactive] public string WinnerUserAccountId { get; set; }
        [Reactive] public string WinnerPhoto { get; set; }
        [Reactive] public bool ShowedWinner { get; set; }
        [Reactive] public bool CanExecutePrizeDraw { get; set; }


        [Reactive] public bool Deleted { get; set; }

        public Stream UploadStream { get; set; }

        public ILocalize Localize { get; set; }

        private readonly ObservableAsPropertyHelper<bool> hasChanged;
        public bool HasChanged => this.hasChanged.Value;

        private readonly ObservableAsPropertyHelper<bool> hasWinner;
        public bool HasWinner => this.hasWinner.Value;

        public PrizeDTO DTO { get; set; }

        public PrizeVM(PrizeDTO prize, ILocalize localize)
        {
            this.Localize = localize;
            if (prize != null)
            {
                this.Id = prize.Id;
                this.Name = prize.Name;
                this.Photo = prize.Photo;
                this.SurveyId = prize.SurveyId;
                this.WinnerName = prize.WinnerName;
                this.WinnerUserAccountId = prize.WinnerUserAccountId;
                this.WinnerPhoto = prize.WinnerPhoto;

                this.DTO = prize;


            }
            hasChanged = this.WhenAny(x => x.Name, x => x.Photo,
                            (name, photo) =>
                            ((this.DTO == null ? String.Empty : this.DTO.Name) != name.Value)
                            ||
                            (this.DTO == null ? null : this.DTO.Photo) != photo.Value)
                             .ToProperty(this, x => x.HasChanged);

            hasWinner = this.WhenAny(x => x.WinnerUserAccountId,
                            (winner) =>
                             winner.Value != null ? true : false)
                             .ToProperty(this, x => x.HasWinner);
        }

        public PrizeDTO ToDTO()
        {
            return new PrizeDTO()
            {
                Id = this.Id,
                Name = this.Name,
                Photo = this.Photo,
                SurveyId = this.SurveyId,
                Deleted = this.Deleted
            };
        }
    }
}
