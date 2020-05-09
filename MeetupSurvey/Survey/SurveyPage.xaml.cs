using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Lottie.Forms;
using MeetupSurvey.Theming;
using ReactiveUI;
using Xamanimation;
using Xamarin.Forms;
using static MeetupSurvey.Survey.SurveyViewModel;

namespace MeetupSurvey.Survey
{
    public partial class SurveyPage : ContentPage
    {
        int height;
        public SurveyPage(IPlatformStyling platformStyling)
        {
            InitializeComponent();
            height = platformStyling.GetStatusBarHeight();
            layout.Padding = new Thickness(0, height, 0, 10);
            header.Margin = new Thickness(0, (height) * -1, 0, 0);
            loading.Margin = new Thickness(0, height + 15, 20, 0);
        }

        private async void Star_OnClicked(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            image.Scale = 1.2;
            await Task.Delay(200);
            image.Scale = 1.0;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext != null)
            {
                (this.BindingContext as SurveyViewModel).WhenAnyValue(x => x.IsBusy).Skip(1).Subscribe(async busy =>
                {
                    switch (busy)
                    {
                        case true:
                            await (this.Resources["headerFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["navBarFadeInAnimation"] as FadeInAnimation).Begin();
                            break;

                        case false:
                            await Task.Delay(400);
                            carousel.IsVisible = true;
                            await Task.Delay(200);
                            await (this.Resources["carouselFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["titleLabelFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();

                            break;
                    }
                });
            }
            
        }

        protected override void OnAppearing()
        {
            App.MasterDetail.IsGestureEnabled = false;
        }

        protected override void OnDisappearing()
        {
            App.MasterDetail.IsGestureEnabled = true;
        }
    }
}
