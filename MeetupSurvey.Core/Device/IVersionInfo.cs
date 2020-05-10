using System;
using System.Collections.Generic;
using System.Text;

namespace MeetupSurvey.Core.Device
{
    public interface IVersionInfo
    {
        string CurrentVersion { get; }
    }
}
