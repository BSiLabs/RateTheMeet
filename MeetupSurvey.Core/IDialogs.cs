using System;
using System.Collections.Generic;
using System.Reactive;

namespace MeetupSurvey.Core

{
    public interface IDialogs
    {
        IObservable<Unit> Alert(string title, string message, string ok = null);
        IObservable<bool> Confirm(string title, string message, string ok = null, string cancel = null);
        IObservable<string> ActionSheet(string title, List<ActionItem> actionItems);
        IObservable<ContextMenuAction> ContextAction(List<ActionItem> actionItems, VerticalOptions verticalOptions);
        IObservable<string> Toast(string error, string message);
    }

    public enum VerticalOptions { Start, Center, End }
}
