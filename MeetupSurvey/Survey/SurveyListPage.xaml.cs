using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using ReactiveUI;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class SurveyListPage : ContentPage
    {
        public SurveyListPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if(this.BindingContext != null)
            (this.BindingContext as SurveyListViewModel).WhenAnyValue(x => x.PageState).
                Subscribe(async (status) => {
                    await Task.Delay(100);
                    switch (status)
                    {
                        case SurveyListPageState.HideButton:
                            await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
                            break;
                        case SurveyListPageState.ShowList:
                            await Task.Delay(600);
                            await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                            await (this.Resources["listFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();
                            break;
                        case SurveyListPageState.ShowButton:
                            await (this.Resources["buttonFadeBackInAnimation"] as FadeToAnimation).Begin();
                            break;
                    }
                });
        }

        async void Handle_Success(object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
        {
            ((sender as CachedImage).BindingContext as SurveyGroup).ImageLoaded = true;
            await ((sender as CachedImage).Resources["imageFadeToAnimation"] as FadeToAnimation).Begin();
        }

        protected override void OnAppearing()
        {
            App.MasterDetail.IsGestureEnabled = true;
        }

        protected override void OnDisappearing()
        {
            App.MasterDetail.IsGestureEnabled = false;
        }
    }
}
