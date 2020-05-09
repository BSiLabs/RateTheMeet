using System;
using System.Windows.Input;
using ReactiveUI;

namespace MeetupSurvey.Dialogs
{
    public class AlertViewModel : ReactiveObject
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public bool IsCancelVisible => this.Cancel != null;
        public string CancelText { get; set; }
        public ICommand Cancel { get; set; }

        public string OkText { get; set; }
        public ICommand Ok { get; set; }
    }
}
