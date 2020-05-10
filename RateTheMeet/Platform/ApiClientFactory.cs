using MeetupSurvey.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateTheMeet.Platform
{
    public class ApiClientFactory
    {
        readonly object syncLock = new object();
        readonly IAppSettings appSettings;
        readonly IProfile profile;


        public ApiClientFactory(IAppSettings appSettings, IProfile profile)
        {
            this.appSettings = appSettings;
            this.profile = profile;
        }


        //IApiClient instance;
        //public IApiClient Instance
        //{
        //    get
        //    {
        //        if (this.instance == null)
        //        {
        //            lock (this.syncLock)
        //            {
        //                if (this.instance == null)
        //                {
        //                    var builder = new HttpHandlerBuilder(new HttpTracerHandler());
        //                    builder.AddHandler(new ApiHttpClientHandler(this.profile, this.appSettings));

        //                    var httpClient = new HttpClient(builder.Build())
        //                    {
        //                        BaseAddress = new Uri(this.appSettings.BaseApiUri)
        //                    };
        //                    this.instance = RestService.For<IApiClient>(httpClient);
        //                }
        //            }
        //        }
        //        return this.instance;
        //    }
        //}


        //public void Start()
        //{
        //    this.appSettings
        //        .RxWhenAnyValue(x => x.BaseApiUri)
        //        .Subscribe(_ =>
        //        {
        //            lock (this.syncLock)
        //                this.instance = null;
        //        });
        //}
    }
}


