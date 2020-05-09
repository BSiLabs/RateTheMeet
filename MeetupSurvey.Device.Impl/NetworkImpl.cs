using System;
using System.Reactive.Linq;
using MeetupSurvey.Core.Device;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;

namespace MeetupSurvey.Device.Impl
{
    public class NetworkImpl : INetwork
    {
        public bool IsAvailable => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public IObservable<bool> WhenStatusChanged() => Observable.Create<bool>(ob =>
        {
            var handler = new EventHandler<ConnectivityChangedEventArgs>(
                (sender, args) => ob.OnNext(args.NetworkAccess == NetworkAccess.Internet));

            Connectivity.ConnectivityChanged += handler;

            return () => Connectivity.ConnectivityChanged -= handler;
        })
        .StartWith(this.IsAvailable);
    }
}
