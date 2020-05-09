using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace MeetupSurvey.Dialogs
{
    public partial class ContextMenuPage : PopupPage
    {
        public ContextMenuPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            (this.BindingContext as ContextMenuViewModel).Close.Execute(null);
        }
    }
}
