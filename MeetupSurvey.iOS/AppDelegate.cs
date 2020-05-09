using System;
using Autofac;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using MeetupSurvey.Core.Data;
using MeetupSurvey.Core.Device;
using MeetupSurvey.iOS.Services;
using MeetupSurvey.Theming;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using PanCardView.iOS;
using Prism;
using Prism.Ioc;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace MeetupSurvey.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UNUserNotificationCenter.Current.Delegate = this;
            Push.PushNotificationReceived += async (sender, e) =>
            {
                var notif = App.Container.Resolve<INotifications>();
                await notif.Send(e.Title, e.Message, e.CustomData);
            };

            global::Xamarin.Forms.Forms.Init();
            FormsMaterial.Init();
            AnimationViewRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();
            CardsViewRenderer.Preserve();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            CarouselView.FormsPlugin.iOS.CarouselViewRenderer.Init();

            //global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);

        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var accountService = (App.Container).Resolve<IAccountService>();
            accountService.ProcessUri(new Uri(url.AbsoluteString));

            return true;
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            uiApplication.ApplicationIconBadgeNumber = 0;
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
Action<UNNotificationPresentationOptions> completionHandler)
        {
            Console.WriteLine("Handling iOS 11 foreground notification");
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action
        completionHandler)
        {
            completionHandler();
        }

        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    var result = Push.DidReceiveRemoteNotification(userInfo)
        //        ? UIBackgroundFetchResult.NewData
        //        : UIBackgroundFetchResult.NoData;

        //    completionHandler(result);
        //}
    }


    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.RegisterSingleton<IPlatformStyling, IOSPlatformStyling>();
            containerRegistry.RegisterSingleton<INotifications, NotificationsImpl>();
        }
    }
}
