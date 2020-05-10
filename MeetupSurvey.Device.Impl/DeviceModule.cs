using System;
using Acr.Settings;
using Autofac;
using MeetupSurvey.Core;
using MeetupSurvey.Core.Device;

namespace MeetupSurvey.Device.Impl
{
    public class DeviceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<NetworkImpl>().As<INetwork>().SingleInstance();

            builder
                .RegisterType<ProfileImpl>()
                .As<IProfile>()
                .SingleInstance();

            builder
                .Register(_ => CrossSettings.Current.Bind<AppSettings>())
                .As<IAppSettings>()
                .SingleInstance();

            builder
                .RegisterType<VersionInfo>().As<IVersionInfo>().SingleInstance();
        }
    }
}
