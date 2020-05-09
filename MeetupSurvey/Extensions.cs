using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI.Legacy;

namespace MeetupSurvey
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservable<T> (this IEnumerable<T> list)
        {
            var returnList = new ObservableCollection<T>();
            foreach (var item in list)
                returnList.Add(item);

            return returnList;
        }

        public static ReactiveList<T> ToReactiveList<T>(this IEnumerable<T> list)
        {
            var returnList = new ReactiveList<T>() { ChangeTrackingEnabled = true };
            foreach (var item in list)
                returnList.Add(item);

            return returnList;
        }

    }
}