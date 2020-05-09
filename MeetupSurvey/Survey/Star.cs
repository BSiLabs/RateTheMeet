using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeetupSurvey.Survey
{
    public class Star : ReactiveObject
    {
        [Reactive] public int Index { get; set; }
        [Reactive] public bool IsOn { get; set; }
        [Reactive] public string Image { get; set; }


        public Star(bool isOn, int index, string image)
        {
            Index = index;
            IsOn = isOn;
            Image = image;
        }
    }
}
