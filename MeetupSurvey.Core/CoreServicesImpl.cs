using MeetupSurvey.Core.Device;

namespace MeetupSurvey.Core
{
    public class CoreServicesImpl : ICoreServices
    {
        public CoreServicesImpl(IProfile profile,
                                IDialogs dialogs,
                                ILocalize localize,
                                ILogger logger,
                                INetwork network,
                                IAppSettings appSettings,
                                IVersionInfo versionInfo,
                                IWebNavigationService webNavigationService)
        {
            this.AppSettings = appSettings;
            this.Profile = profile;
            this.Dialogs = dialogs;
            this.Localize = localize;
            this.Network = network;
            this.Log = logger;
            this.VersionInfo = versionInfo;
            this.WebNavigationService = webNavigationService;
        }


        public IAppSettings AppSettings { get; }
        public IProfile Profile { get; }
        public IDialogs Dialogs { get; }
        public ILocalize Localize { get; }
        public INetwork Network { get; }
        public ILogger Log { get; }
        public IVersionInfo VersionInfo { get; }
        public IWebNavigationService WebNavigationService { get; }
    }
}
