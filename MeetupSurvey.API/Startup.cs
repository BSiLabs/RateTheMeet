using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using Refit;
using MeetupSurvey.API.Services;
using Newtonsoft.Json;
using MeetupSurvey.Services.PushNotifications;
using MeetupSurvey.API.ConfigOptions;

namespace MeetupSurvey.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables();
            //Configuration = builder.Build();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            //services.AddDbContext<MeetupSurveyContext>(opt =>
            //    opt.UseInMemoryDatabase("SurveyList"));
            var connection = Configuration.GetConnectionString("SqlConnectionString");

            services.AddDbContext<MeetupSurveyContext>
                (options => options.UseSqlServer(connection));

            services.AddRefitClient<IMeetupAuth>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Constants.BaseMeetupSecureUri));

            services.AddRefitClient<IMeetupClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Constants.BaseMeetupUri));

            var cfg = this.Configuration.GetSection("AppCenter");

            var push = new AppCenterPushNotifications()
            {
                ApiKey = cfg["ApiKey"],
                OrganizationName = cfg["OrganizationName"],
                iOSAppName = cfg["iOSAppName"],
                AndroidAppName = cfg["AndroidAppName"]
            };

            //If we are debugging locally we can not replace token, hard code app names instead
            if (push.iOSAppName == "#{iOSAppName}#")
                push.iOSAppName = "MeetupSurvey-Debug";

            if (push.AndroidAppName == "#{AndroidAppName}#")
                push.AndroidAppName = "RateTheMeet-Debug";
            ////////////////


            services.AddSingleton<IPushNotifications>(push);
            services.Configure<AppSettings>(Configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => {
                    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
        
    }
}
