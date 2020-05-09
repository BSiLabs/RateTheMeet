using System;
using System.Net.Http;
using Autofac;
using HttpTracer;
using MeetupSurvey.ApiClient;
using MeetupSurvey.Core;
using Refit;
using System.Diagnostics;
using MonkeyCache.SQLite;
using Acr.Reactive;

namespace MeetupSurvey.Data.Impl
{
    public class ApiClientFactory : IStartable
    {
        readonly object syncLock = new object();
        readonly IAppSettings appSettings;
        readonly IProfile profile;


        public ApiClientFactory(IAppSettings appSettings, IProfile profile)
        {
            this.appSettings = appSettings;
            this.profile = profile;
            Barrel.ApplicationId = "RateTheMeet";
        }


        IApiClient instance;
        public IApiClient Instance
        {
            get
            {
                if (this.instance == null)
                {
                    lock (this.syncLock)
                    {
                        if (this.instance == null)
                        {
                            var builder = new HttpHandlerBuilder(new HttpTracerHandler());
                            builder.AddHandler(new ApiHttpClientHandler(this.profile, this.appSettings));

                            var httpClient = new HttpClient(builder.Build())
                            {
                                BaseAddress = new Uri(this.appSettings.BaseApiUri)
                            };
                            this.instance = RestService.For<IApiClient>(httpClient);
                        }
                    }
                }
                return this.instance;
            }
        }


        public void Start()
        {
            this.appSettings
                .RxWhenAnyValue(x => x.BaseApiUri)
                .Subscribe(_ =>
                {
                    lock (this.syncLock)
                        this.instance = null;
                });
        }
    }
}
