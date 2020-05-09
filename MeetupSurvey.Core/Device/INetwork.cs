using System;
namespace MeetupSurvey.Core.Device
{
    public interface INetwork
    {
        bool IsAvailable { get; }
        IObservable<bool> WhenStatusChanged();
    }
}
