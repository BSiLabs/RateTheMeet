using System;
namespace MeetupSurvey.ApiClient
{
    public class MeetupAuthArgs
    {    
        public string client_id { get; set; }
        public string response_type { get; set; }
        public string redirect_uri { get; set; }

        public MeetupAuthArgs(string clientid, string responsetype, string redirecturi)
        {
            this.client_id = clientid;
            this.response_type = responsetype;
            this.redirect_uri = redirecturi;
        }

    }
}
