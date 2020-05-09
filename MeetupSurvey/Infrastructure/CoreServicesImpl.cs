using System;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Device;

namespace MeetupSurvey.Infrastructure
{
    public class CoreServicesImpl : ICoreServices
    {
        public CoreServicesImpl(IProfile profile,
                                IDialogs dialogs,
                                ILocalize localize,
                                ILogger logger,
                                INetwork network,
                                IAppSettings appSettings)
        {
            this.AppSettings = appSettings;
            this.Profile = profile;
            this.Dialogs = dialogs;
            this.Localize = localize;
            this.Network = network;
            this.Log = logger;
        }


        public IAppSettings AppSettings { get; }
        public IProfile Profile { get; }
        public IDialogs Dialogs { get; }
        public ILocalize Localize { get; }
        public INetwork Network { get; }
        public ILogger Log { get; }
    }
}
