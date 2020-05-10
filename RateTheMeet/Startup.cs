using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using MeetupSurvey.Core;
using RateTheMeet.Platform;
using MeetupSurvey.Core.Device;
using MeetupSurvey.Core.Data;

namespace RateTheMeet
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                });

            services.AddTransient<ICoreServices, CoreServicesImpl>();
            services.AddSingleton<IProfile, Profile>();
            services.AddTransient<IDialogs, Dialogs>();
            services.AddTransient<ILocalize, Localize>();
            services.AddTransient<ILogger, Log>();
            services.AddTransient<INetwork, Network>();
            services.AddTransient<IAppSettings, AppSettings>();
            services.AddTransient<IWebNavigationService, WebNavigationService>();
            
            services.AddSingleton<IVersionInfo, VersionInfo>();
            services.AddSingleton<ISurveyService, SurveyService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
