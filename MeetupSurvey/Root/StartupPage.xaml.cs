using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Root
{
    public partial class StartupPage : ContentPage
    {
        public StartupPage()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            await (this.Resources["loadingFadeInAnimation"] as FadeInAnimation).Begin();
            await (this.Resources["logoFadeInAnimation"] as TranslateToAnimation).Begin();
            await (this.Resources["bsiLogoFadeInAnimation"] as FadeInAnimation).Begin();

            //Allows a smoother transition
            await Task.Delay(350);
            if (this.BindingContext != null)
                (this.BindingContext as StartupViewModel).ShouldNavigate = true;
        }
    }
}
