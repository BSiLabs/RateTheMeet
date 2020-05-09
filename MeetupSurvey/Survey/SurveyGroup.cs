using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MeetupSurvey.DTO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public class SurveyGroup : ObservableCollection<SurveyVM>, INotifyPropertyChanged
    {
        public string GroupId { get; set; }
        [Reactive] public string GroupName { get; set; }
        [Reactive] public string KeyPhoto { get; set; }
        [Reactive] public string GroupPhoto { get; set; }
        [Reactive] public string GroupLink { get; set; }

        public string OrganizerName { get; set; }
        public string OrganizerPhoto { get; set; }

        [Reactive] public string EventName { get; set; }
        [Reactive] public string EventTime { get; set; }
        [Reactive] public string EventLink { get; set; }

        [Reactive] public bool GroupSubscribed { get; set; }


        public bool IsAdmin { get; set; }
        public bool NoSurveys { get; set; }

        public bool imageLoaded { get; set; }

        public bool ImageLoaded
        {
            set
            {
                if (imageLoaded != value)
                {
                    imageLoaded = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ImageLoaded"));
                    }
                }
            }
            get
            {
                return imageLoaded;
            }
        }

        public ICommand GoToWebsite => ReactiveCommand.CreateFromTask(async () =>
         {
             await Xamarin.Essentials.Browser.OpenAsync("https://www.meetup.com/groups/");
         });

        public ICommand GoToLink => ReactiveCommand.CreateFromTask(async () =>
        {
            await Xamarin.Essentials.Browser.OpenAsync(this.EventLink ?? this.GroupLink);
        });

        public SurveyGroup(GroupDTO group, bool noSurveys = false)
        {
            if (group != null)
            {
                NoSurveys = noSurveys;
                GroupId = group.Id;
                if (NoSurveys)
                    GroupName = group.Name;
                else
                    GroupName = group.Name.ToUpper();
                KeyPhoto = group.KeyPhoto;
                GroupPhoto = group.GroupPhoto;
                if (group.Organizers != null && group.Organizers.Count > 0)
                {
                    OrganizerName = group.Organizers[0].Name;
                    OrganizerPhoto = group.Organizers[0].Photo;
                }
                IsAdmin = group.IsAdmin;
                EventName = group.EventName;
                EventTime = group.EventTime;
                GroupSubscribed = group.GroupSubscribed;
                GroupLink = group.Link;
                EventLink = group.EventLink;
                if (KeyPhoto == null)
                    ImageLoaded = true;
            }
        }

        public SurveyGroup(GroupDTO group, IEnumerable<SurveyVM> items) : this (group)
        {
            if(items != null)
            foreach (var item in items)
                this.Items.Add(item);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void Update(SurveyGroup surveyGroup)
        {
            this.GroupName = surveyGroup.GroupName;
            this.GroupPhoto = surveyGroup.GroupPhoto;
            this.KeyPhoto = surveyGroup.KeyPhoto;
            this.EventTime = surveyGroup.EventTime;
            this.EventName = surveyGroup.EventName;
            this.OrganizerName = surveyGroup.OrganizerName;
            this.OrganizerPhoto = surveyGroup.OrganizerPhoto;
            this.EventLink = surveyGroup.EventLink;
            this.GroupLink = surveyGroup.GroupLink;
        }
    }
}