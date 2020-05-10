using MeetupSurvey.Core;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace RateTheMeet.Platform
{
    public class Dialogs : IDialogs
    {
        public IObservable<string> ActionSheet(string title, List<ActionItem> actionItems)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> Alert(string title, string message, string ok = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> Confirm(string title, string message, string ok = null, string cancel = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<ContextMenuAction> ContextAction(List<ActionItem> actionItems, VerticalOptions verticalOptions)
        {
            throw new NotImplementedException();
        }

        public IObservable<string> Toast(string error, string message)
        {
            throw new NotImplementedException();
        }
    }
}
