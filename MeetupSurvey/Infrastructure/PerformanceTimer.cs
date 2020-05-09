using System;
using System.Diagnostics;

namespace MeetupSurvey.Infrastructure
{
    public class PerformanceTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _eventName;
        
        public PerformanceTimer(string eventName)
        {
            _eventName = eventName;
            _stopwatch = Stopwatch.StartNew();
            Debug.WriteLine($"Performance Timer - {_eventName} : Start");
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            Debug.WriteLine($"Performance Timer - {_eventName} : Completed : {_stopwatch.ElapsedMilliseconds}ms");
        }
    }
}