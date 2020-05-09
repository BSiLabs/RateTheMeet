
using ReactiveUI;

namespace MeetupSurvey.Dialogs
{
    public class ToastViewModel : ReactiveObject
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }
}