using System;
namespace MeetupSurvey.Core
{
    public interface ILocalize
    {
        string this[string key] { get; }
        string GetEnumValue(Enum value);
    }
}
