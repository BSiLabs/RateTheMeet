using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MeetupSurvey.Theming;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace MeetupSurvey.Root
{
    
    public partial class MainPage : MasterDetailPage
    {
       

        public new bool IsGestureEnabled
        {
            get => base.IsGestureEnabled;
            set
            {
                if(Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                    base.IsGestureEnabled = value;
            }
        }

        IPlatformStyling platformStyling;
        public MainPage(IPlatformStyling platformStyling)
        {
        
            InitializeComponent();
            this.platformStyling = platformStyling;
            App.MasterDetail = this;

            //App.MasterDetail.IsGestureEnabled = true;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            (this.BindingContext as MainViewModel).AdminToggled.Execute(e.Value);
        }

        void Handle_LayoutChanged(object sender, System.EventArgs e)
        {
            var height = this.platformStyling.GetStatusBarHeight() + 5;
            (sender as Grid).Padding = new Thickness(0, height + 30, 0, 0);
        }
    }
}
