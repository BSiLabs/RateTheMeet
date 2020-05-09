using System;
using System.Resources;
using MeetupSurvey.Core;

namespace MeetupSurvey.Infrastructure
{
    public class Localize : ILocalize
    {
        readonly ResourceManager resourceManager;

        public Localize()
        {
            this.resourceManager = new ResourceManager("MeetupSurvey.Resources.AppResources", this.GetType().Assembly);
        }

        public string this[string key] => this.resourceManager.GetString(key);

        public string GetEnumValue(Enum value)
        {
            var key = value.GetType().Name + value;
            return this[key];
        }
    }
}