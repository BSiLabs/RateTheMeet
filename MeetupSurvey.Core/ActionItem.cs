using System.Windows.Input;

namespace MeetupSurvey.Core
{
    public class ActionItem
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Icon { get; set; }
        public bool ShowSeparator { get; set; }
        public ContextAction ContextAction { get; set; }
        public ICommand Action { get; set; }
    }
}