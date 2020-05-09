using System;
using System.Collections.Generic;
using MeetupSurvey.Theming;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Account
{
    public partial class LoginPage : ContentPage
    {
        bool loaded = false;
        public LoginPage(IPlatformStyling platFormStyling)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override async void OnAppearing()
        {
            if (!loaded)
            {
                await (this.Resources["logoFadeInAnimation"] as FadeInAnimation).Begin();
                await (this.Resources["buttonFadeInAnimation"] as FadeInAnimation).Begin();
                await (this.Resources["bsiLogoFadeInAnimation"] as FadeInAnimation).Begin();
                loaded = true;
            }
        }
    }
}
