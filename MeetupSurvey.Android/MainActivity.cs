using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using MeetupSurvey.Theming;
using Xamarin.Forms;
using Lottie.Forms.Droid;
using Rg.Plugins.Popup.Extensions;
//using Xamarin.Auth;
using Plugin.CurrentActivity;
using PanCardView.Droid;
using MeetupSurvey.Core.Device;
using MeetupSurvey.Droid.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Autofac;
using System.Collections.Generic;

namespace MeetupSurvey.Droid
{
    [Activity(Label = "MeetupSurvey", Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_round_launcher", Theme="@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    //[IntentFilter(new[] { Intent.ActionView },
    //Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //DataScheme = "meetupsurvey")] 
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
                PrismApplicationBase.Current.MainPage.Navigation.PopAllPopupAsync();
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            PlatformStyling.Activity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

        base.OnCreate(bundle);

            //global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            //CustomTabsConfiguration.CustomTabsClosingMessage = null;
            global::Xamarin.Forms.Forms.Init(this, bundle);
            FormsMaterial.Init(this, bundle);
            CardsViewRenderer.Preserve();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);
            AnimationViewRenderer.Init();
            CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);


            ////-----------------------------------------------------------------------------------------------
            //// Xamarin.Auth initialization

            //// Presenters Initialization
            //global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);

            //// User-Agent tweaks for Embedded WebViews
            //global::Xamarin.Auth.WebViewConfiguration.Android.UserAgent = "moljac++";

            //// Xamarin.Auth CustomTabs Initialization/Customisation
            //global::Xamarin.Auth.CustomTabsConfiguration.ActionLabel = null;
            //global::Xamarin.Auth.CustomTabsConfiguration.MenuItemTitle = null;
            //global::Xamarin.Auth.CustomTabsConfiguration.AreAnimationsUsed = true;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsShowTitleUsed = false;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsUrlBarHidingUsed = false;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsCloseButtonIconUsed = false;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsActionButtonUsed = false;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsActionBarToolbarIconUsed = false;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsDefaultShareMenuItemUsed = false;

            //global::Android.Graphics.Color color_xamarin_blue;
            //color_xamarin_blue = new global::Android.Graphics.Color(0x34, 0x98, 0xdb);
            //global::Xamarin.Auth.CustomTabsConfiguration.ToolbarColor = color_xamarin_blue;


            //// ActivityFlags for tweaking closing of CustomTabs
            //// please report findings!
            //global::Xamarin.Auth.CustomTabsConfiguration.
            //   ActivityFlags =
            //        global::Android.Content.ActivityFlags.NoHistory
            //        |
            //        global::Android.Content.ActivityFlags.SingleTop
            //        |
            //        global::Android.Content.ActivityFlags.NewTask
            //        ;

            //global::Xamarin.Auth.CustomTabsConfiguration.IsWarmUpUsed = true;
            //global::Xamarin.Auth.CustomTabsConfiguration.IsPrefetchUsed = true;

            //global::Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;

            ////-----------------------------------------------------------------------------------------------



            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            //{
            //    Window.DecorView.SystemUiVisibility = 0;
            //    //var statusBarHeightInfo = typeof(FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //    //statusBarHeightInfo.SetValue(this, 0);
            //    Window.SetStatusBarColor(new Android.Graphics.Color(255, 255, 255, 50));
            //}




            ////this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            //// add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
            //this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);


            //this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            //this.Window.SetStatusBarColor(new Android.Graphics.Color(255, 255, 255, 50));


            LoadApplication(new App(new AndroidInitializer()));

            //App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.RegisterSingleton<IPlatformStyling, PlatformStyling>();
            containerRegistry.RegisterSingleton<INotifications, NotificationsImpl>();
        }
    }
}

