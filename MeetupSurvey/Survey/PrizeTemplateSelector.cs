using System;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public class PrizeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PendingTemplate { get; set; }
        public DataTemplate WinTemplate { get; set; }
        public DataTemplate LoseTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // TODO check if organizer has closed survey, if they haven't = PendingTemplate
            // if they have closed survey, check if user won/lost
            return PendingTemplate;
        }
    }
}
