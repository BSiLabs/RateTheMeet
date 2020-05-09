using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MeetupSurvey.Root
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.MasterDetail.IsGestureEnabled = true;
        }
    }
}
