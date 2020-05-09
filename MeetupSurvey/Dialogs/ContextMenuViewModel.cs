using System;
using System.Collections.Generic;
using System.Windows.Input;
using MeetupSurvey.Core;
using ReactiveUI;
using Xamarin.Forms;

namespace MeetupSurvey.Dialogs
{
    public class ContextMenuViewModel : ReactiveObject
    {
       
        public string Title { get; set; }
        public LayoutOptions VerticalPlacement { get; set; }
        public List<ActionItem> ItemList { get; set; }
        public ICommand Close { get; set; }
    }


}
