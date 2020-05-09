using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public class QuestionVM : ReactiveObject
    {
        public string Id { get; set; }
        public string SurveyId { get; set; }
        [Reactive] public string Name { get; set; } = String.Empty;
        [Reactive] public bool IsSelected { get; set; }
        [Reactive] public int Order { get; set; }
        [Reactive] public int Rating { get; set; } = 0;
        [Reactive] public string Comment { get; set; }
        [Reactive] public bool HasLoaded { get; set; }

        [Reactive] public bool CanSwipe { get; set; }
        [Reactive] public bool Deleted { get; set; }

        private readonly ObservableAsPropertyHelper<bool> hasChanged;
        public bool HasChanged => this.hasChanged.Value;

        public QuestionDTO DTO { get; set; }

        public ILocalize Localize { get; set; }

        private readonly string _image = "emptystar.png";

        public QuestionVM(QuestionDTO question, ILocalize localize)
        {
            this.Localize = localize;

            if (question != null)
            {
                this.Id = question.Id;
                this.Name = question.Name;
                this.Order = question.Order;
                this.SurveyId = question.SurveyId;
                this.DTO = question;
            }

            StarList = new ObservableCollection<Star>
            {
                new Star(false, 1, _image),
                new Star(false, 2, _image),
                new Star(false, 3, _image),
                new Star(false, 4, _image),
                new Star(false, 5, _image)
            };

            hasChanged = this.WhenAny(x => x.Name, (name) => (this.DTO == null ? String.Empty : this.DTO.Name) != name.Value)
                             .ToProperty(this, x => x.HasChanged);
        }

        public QuestionDTO ToDTO()
        {
            return new QuestionDTO()
            {
                Id = this.Id,
                Name = this.Name,
                Order = this.Order,
                SurveyId = this.SurveyId,
                Deleted = Deleted
            };
        }

        public ObservableCollection<Star> StarList { get; set; }
        [Reactive] public bool IsEnd { get; set; } = false;
        [Reactive] public string CompletionText { get; internal set; }

        public ICommand RatingSelected { get; set; }
        public ICommand Next { get; set; }
        public ICommand Previous { get; set; }

    }
}
