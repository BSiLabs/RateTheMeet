using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class MyGroupsPage : ContentPage
    {
        public MyGroupsPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext != null)
                (this.BindingContext as MyGroupsViewModel).WhenAnyValue(x => x.PageState).
                    Subscribe(async (status) =>
                    {
                        switch(status)
                        {
                            case MyGroupsListPageState.Done:
                                await Task.Delay(600);
                                await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                                await (this.Resources["listFadeInAnimation"] as FadeInAnimation).Begin();
                                break;
                        }
                    });
        }
        protected override void OnAppearing()
        {
            App.MasterDetail.IsGestureEnabled = true;
        }
    }
}
