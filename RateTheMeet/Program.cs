using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Core.Device;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RateTheMeet.Platform;

namespace RateTheMeet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                });

            builder.Services.AddTransient<ICoreServices, CoreServicesImpl>();
            builder.Services.AddTransient<IDialogs, Dialogs>();
            builder.Services.AddTransient<ILocalize, Localize>();
            builder.Services.AddTransient<ILogger, Log>();
            builder.Services.AddTransient<INetwork, Network>();
            builder.Services.AddTransient<IAppSettings, AppSettings>();
            builder.Services.AddTransient<IWebNavigationService, WebNavigationService>();
            ;
            builder.Services.AddSingleton<IProfile, Profile>();
            builder.Services.AddSingleton<IVersionInfo, VersionInfo>();
            builder.Services.AddSingleton<ISurveyService, MeetupSurvey.Data.Impl.Mocks.SurveyService>();

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }

        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        //public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
        //    BlazorWebAssemblyHost.CreateDefaultBuilder()
        //        .UseBlazorStartup<Startup>();
    }
}
