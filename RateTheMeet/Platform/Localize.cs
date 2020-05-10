using MeetupSurvey.Core;
using System;

namespace RateTheMeet.Platform
{
    public class Localize : ILocalize
    {
        public string this[string key] => throw new NotImplementedException();

        public string GetEnumValue(Enum value)
        {
            throw new NotImplementedException();
        }
    }
}
