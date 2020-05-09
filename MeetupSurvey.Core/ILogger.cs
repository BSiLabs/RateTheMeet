using System;
using System.Collections.Generic;

namespace MeetupSurvey.Core
{
    public interface ILogger
    {
        void WriteCrash(Exception exception, IDictionary<string, string> contextParameters = null);
        void WriteEvent(string eventName, IDictionary<string, string> contextParameters = null);
    }
}
