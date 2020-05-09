using System;
using Autofac;
using MeetupSurvey.Core;
using MeetupSurvey.Infrastructure;
using MeetupSurvey.Data.Impl;
using MeetupSurvey.Device.Impl;
using Plugin.Jobs;
using MeetupSurvey.Dialogs;
using MeetupSurvey.Core.Device;
using MeetupSurvey.Core.Data;

namespace MeetupSurvey
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<DataModule>();
            builder.RegisterModule<DeviceModule>();

            builder.RegisterType<CoreServicesImpl>().As<ICoreServices>().SingleInstance();
            builder.RegisterType<MeetupSurvey.Logging.AppCenter.AppCenterLogger>().As<ILogger>().SingleInstance();

#if DEBUG
            //builder.RegisterType<TraceLogListener>().As<IStartable>().AutoActivate().SingleInstance();
#endif
            builder.RegisterType<GlobalExceptionHandler>().As<IStartable>().AutoActivate().SingleInstance();
            builder.RegisterType<Localize>().As<ILocalize>().SingleInstance();
            builder.RegisterType<DialogsImpl>().As<IDialogs>().SingleInstance();
            builder.RegisterType<NetworkImpl>().As<INetwork>().SingleInstance();
            builder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();

            builder.RegisterJobManager();
        }
    }
}
