using System;
using System.Windows.Input;

namespace MeetupSurvey.Root
{
    public class MenuItemVM
    {
        public string Icon { get; set; }
        public string FontFamily { get; set; }
        public string Title { get; set; }
        public ICommand Click { get; set; }
        public bool IsVisible { get; set; }

    }
}
