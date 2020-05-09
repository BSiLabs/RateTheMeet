using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.DTO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Native = Microsoft.AppCenter.AppCenter;
using System.Threading.Tasks;

namespace MeetupSurvey.Logging.AppCenter
{
    public class AppCenterLogger : ILogger
    {
        // TODO: make this a secret to be pulled from build
        readonly IDisposable signInSub;
        readonly IDisposable signOutSub;
        readonly IDialogs dialogs;


        public AppCenterLogger(IProfile profile, IDialogs dialogs)
        {
            this.dialogs = dialogs;
            if(Native.Configured)
                Native.Start(
                typeof(Analytics),
                typeof(Crashes));
            else
                Native.Start(
                Constants.AppCenterToken,
                typeof(Analytics),
                typeof(Crashes));
            
            
            this.signInSub = profile
                .WhenUserSignedIn()
                .Subscribe(this.OnSignIn);

            this.signOutSub = profile
                .WhenUserSignedOut()
                .Subscribe(_ => this.OnSignOut());
        }


        void OnSignIn(AuthUser user)
        {
            if (user is null) return;
            Native.SetUserId(user.Email);
        }


        void OnSignOut()
        {
            Native.SetUserId(string.Empty);
        }

        public void WriteCrash(Exception exception, IDictionary<string, string> contextParameters = null)
        {
            if (!KnownExceptions.ContainsKey(nameof(exception)))
            {
                Crashes.TrackError(exception, contextParameters);
#if DEBUG
                this.dialogs.Toast("Error", exception.ToString());
#else
                this.dialogs.Toast("Unhandled Exception", String.Empty); //this.localize["ExceptionUnmanaged"]);
#endif
            }
            else if (KnownExceptions[nameof(exception)] != "")
            {
                this.dialogs.Toast("Error", KnownExceptions[nameof(exception)]);
            }
        }
        

        public void WriteEvent(string eventName, IDictionary<string, string> contextParameters = null)
            => Analytics.TrackEvent(eventName, contextParameters);

        Dictionary<string, string> KnownExceptions => new Dictionary<string, string>()
        {
            { nameof(TaskCanceledException), "" }
        };

    }
}
