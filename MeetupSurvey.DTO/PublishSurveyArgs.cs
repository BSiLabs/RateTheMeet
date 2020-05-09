using System;
namespace MeetupSurvey.DTO
{
    public class PublishSurveyArgs
    {
        public string SurveyId { get; set; }
        public bool Unpublish { get; set; }
        public bool Notify { get; set; }

        public PublishSurveyArgs(string surveyId, bool unpublish, bool notify)
        {
            SurveyId = surveyId;
            Unpublish = unpublish;
            Notify = notify;
        }
    }
}
