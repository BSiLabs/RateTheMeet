using MeetupSurvey.Core;
using System;
using System.Collections.Generic;

namespace RateTheMeet.Platform
{
    public class Log : ILogger
    {
        public void WriteCrash(Exception exception, IDictionary<string, string> contextParameters = null)
        {
            throw new NotImplementedException();
        }

        public void WriteEvent(string eventName, IDictionary<string, string> contextParameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
