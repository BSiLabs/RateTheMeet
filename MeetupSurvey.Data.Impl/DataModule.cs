using System;
using Autofac;
using MeetupSurvey.Core.Data;

namespace MeetupSurvey.Data.Impl
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<UserTasks>().As<IStartable>().AutoActivate().SingleInstance();

            builder.RegisterType<SurveyService>().As<ISurveyService>().SingleInstance();

            builder.RegisterType<MeetupAuthService>().As<IMeetupService>().SingleInstance();

            builder
                .RegisterType<ApiClientFactory>()
                .As<IStartable>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();
        }
    }
}
