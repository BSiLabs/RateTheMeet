using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class ArchiveListPage : ContentPage
    {
        public ArchiveListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            App.MasterDetail.IsGestureEnabled = true;
            (this.BindingContext as ArchiveListViewModel).WhenAnyValue(x => x.PageState).
                Subscribe(async (status) => {
                    switch (status)
                    {
                        case ArchiveListPageState.Done:
                            await Task.Delay(600);
                            await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                            await (this.Resources["listFadeInAnimation"] as FadeInAnimation).Begin();
                            break;
                    }
                });
        }
    }
}
