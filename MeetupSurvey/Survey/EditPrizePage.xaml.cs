using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using MeetupSurvey.Theming;
using ReactiveUI;
using Rg.Plugins.Popup.Extensions;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class EditPrizePage : ContentPage
    {
        public EditPrizePage(IPlatformStyling platformStyling)
        {
            InitializeComponent();

            var height = platformStyling.GetStatusBarHeight() + 5;
            layout.Padding = new Thickness(20, height, 20, 50);
            bgImage.Margin = new Thickness(-20, height * -1, -20, -50);

            //EditButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            //ellipsis.IsEnabled = false;

            App.MasterDetail.IsGestureEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            if ((this.BindingContext is EditPrizeViewModel vm) && vm.HasChanged)
            {
                vm.GoBack.Execute(null);
                return true;
            }
            else
                return false;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext != null)
            {
                var vm = (this.BindingContext as EditPrizeViewModel);

                //EditButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                //ellipsis.IsEnabled = true;


                vm.WhenAnyValue(x => x.Survey).Subscribe(async (survey) =>
                {
                    if (survey != null)
                    {
                        await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();
                        await (this.Resources["stackLayoutFadeInAnimation"] as FadeInAnimation).Begin();
                        await (this.Resources["loadingFadeOutAnimation"] as FadeToAnimation).Begin();
                    }
                }).DisposeWith(vm.DestroyWith);

                vm.WhenAny(x => x.HasSaved, (hasSaved) => hasSaved.Value)
                    .Subscribe(async (hasSaved) =>
                    {
                        if(hasSaved)
                        {
                            await (this.Resources["loadingFadeOutAnimation"] as FadeToAnimation).Begin();
                            await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();

                            vm.HasSaved = false;
                            vm.IsSaving = false;
                        }
                    });

                vm.WhenAny(x => x.IsSaving, (isSaving) => isSaving.Value)
                    .Subscribe(async (isSaving) =>
                    {
                        if (isSaving)
                        {
                            await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
                            await (this.Resources["loadingFadeToAnimation"] as FadeToAnimation).Begin();
                        }
                    });
            }
        }


        private bool ShouldFocus = false;
        private void NewPrize_Clicked(object sender, EventArgs e)
        {
            ShouldFocus = true;
            var vm = (this.BindingContext as EditPrizeViewModel);

            stackLayout.ScrollTo(vm.Survey.PrizeList.Last(), ScrollToPosition.Start, true);
        }
        private async void Entry_Loaded(object sender, EventArgs e)
        {
            Editor editor = (Editor)sender;
            if (!(editor.BindingContext as PrizeVM).HasLoaded)
            {
                (editor.BindingContext as PrizeVM).HasLoaded = true;
                //This Task.Delay is REQUIRED to allow the ListView to scroll the item into place before focusing the Editor
                await Task.Delay(500);
                if (ShouldFocus)
                {
                    editor.Focus();
                    ShouldFocus = false;
                    editor.Focus();
                }
                ShouldFocus = false;
            }
            else if (editor.IsFocused)
            {
                var viewCell = editor.Parent.Parent.Parent.Parent.Parent as ViewCell;
                viewCell.ForceUpdateSize();
            }
        }

        async void Handle_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            ((sender as Editor).BindingContext as PrizeVM).IsSelected = false;
            await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();

        }

        async void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
            ((sender as Editor).BindingContext as PrizeVM).IsSelected = true;

        }
    }
}
