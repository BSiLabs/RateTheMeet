using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Dialogs
{
    public class ActionSheetViewModel : ReactiveObject
    {
        public string Title { get; set; }
        public List<ActionItem> ItemList { get; set; }
        public string CancelText { get; set; }
        public ICommand Cancel{ get; set; }
    }
}
