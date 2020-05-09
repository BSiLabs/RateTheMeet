using System;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public class SurveyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EndTemplate { get; set; }
        public DataTemplate QuestionTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is QuestionVM viewModel)
                return viewModel.IsEnd ? EndTemplate : QuestionTemplate;
            return null;
        }
    }
}
