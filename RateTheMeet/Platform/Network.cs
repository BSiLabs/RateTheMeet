using MeetupSurvey.Core.Device;
using System;

namespace RateTheMeet.Platform
{
    public class Network : INetwork
    {
        public bool IsAvailable => throw new NotImplementedException();

        public IObservable<bool> WhenStatusChanged()
        {
            throw new NotImplementedException();
        }
    }
}
