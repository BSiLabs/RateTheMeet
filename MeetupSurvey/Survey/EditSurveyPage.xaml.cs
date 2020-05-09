using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MeetupSurvey.Core;
using MeetupSurvey.Theming;
using ReactiveUI;
using Rg.Plugins.Popup.Extensions;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class EditSurveyPage : ContentPage
    {
        readonly ILogger logger;
        public EditSurveyPage(IPlatformStyling platformStyling, ILogger logger)
        {
            this.logger = logger;
            InitializeComponent();

            var height = platformStyling.GetStatusBarHeight() + 5;
            layout.Padding = new Thickness(20, height, 20, 50);
            bgImage.Margin = new Thickness(-20, height *-1, -20, -50);

            EditButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            ellipsis.IsEnabled = false;

            App.MasterDetail.IsGestureEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            if ((this.BindingContext is EditSurveyViewModel vm) && vm.HasChanged)
            {
                vm.GoBack.Execute(null);
                return true;
            }
            else
                return false;
           
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if(this.BindingContext != null)
            {
                var vm = (this.BindingContext as EditSurveyViewModel);

                await(this.Resources["layoutFadeInAnimation"] as FadeInAnimation).Begin();
                EditButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                ellipsis.IsEnabled = true;


                vm.SaveSurvey.IsExecuting.Subscribe(async (saving) =>
                {
                    if (saving)
                        await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
                });


                vm.WhenAny(x => x.IsEditing, x => x.IsLoading,
                    (editing, loading) => (editing, loading)
                ).Subscribe(async (state) =>
                {
                    if (state.editing.Value)
                    {
                        await (this.Resources["publishButtonFadeOutAnimation"] as FadeToAnimation).Begin();
                        await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();
                    }
                    else if (state.loading.Value == false)
                    {
                        await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
                        await (this.Resources["publishButtonFadeInAnimation"] as FadeToAnimation).Begin();
                    }

                }).DisposeWith(vm.DestroyWith);


                vm.WhenAnyValue(x => x.Survey).Subscribe(async (survey) =>
                {
                    if (survey != null)
                    {
                        await (this.Resources["stackLayoutFadeInAnimation"] as FadeInAnimation).Begin();
                        await (this.Resources["titleHeaderFadeInAnimation"] as FadeInAnimation).Begin();
                        await (this.Resources["groupLabelFadeInAnimation"] as FadeInAnimation).Begin();
                        await (this.Resources["pageLabelFadeInAnimation"] as FadeInAnimation).Begin();
                        await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                        await (this.Resources["prizeButtonFadeInAnimation"] as FadeInAnimation).Begin();
                    }
                })
                .DisposeWith(vm.DestroyWith);

                vm.WhenAnyValue(x => x.HintEditButton).Subscribe(async (hint) =>
                {
                    if (hint)
                    {
                        await (this.Resources["editButtonScaleUpAnimation"] as ScaleToAnimation).Begin();
                        await (this.Resources["editButtonScaleDownAnimation"] as ScaleToAnimation).Begin();
                        await (this.Resources["editButtonScaleUpAnimation"] as ScaleToAnimation).Begin();
                        await (this.Resources["editButtonScaleDownAnimation"] as ScaleToAnimation).Begin();
                        await (this.Resources["editButtonScaleUpAnimation"] as ScaleToAnimation).Begin();
                        await (this.Resources["editButtonScaleDownAnimation"] as ScaleToAnimation).Begin();
                        vm.HintEditButton = false;
                    }
                })
                .DisposeWith(vm.DestroyWith);
            }
        }

        protected override async void OnAppearing()
        {
            App.MasterDetail.IsGestureEnabled = false;
        }

        private bool ShouldFocus = false;
        private void NewQuestion_Clicked(object sender, EventArgs e) => ShouldFocus = true;

        private async void Entry_Loaded(object sender, EventArgs e)
        {
            try
            {
                Editor editor = (Editor)sender;
                if (!(editor.BindingContext as QuestionVM).HasLoaded)
                {
                    (editor.BindingContext as QuestionVM).HasLoaded = true;
                    await Task.Delay(200);
                    if (ShouldFocus)
                    {
                        editor.Focus();
                        ShouldFocus = false;
                        await scrollQuestions.ScrollToAsync(editor, ScrollToPosition.Start, true);
                        editor.Focus();
                    }
                    ShouldFocus = false;
                }
            }
            catch (Exception ex)
            {
                //Happens when editor is no longer in view, might not even need to log the crash here as there is nothing to really do about it
                logger.WriteCrash(ex);
            }
        }

        async void Handle_Unfocused(object sender, FocusEventArgs e)
        {
            //((sender as Editor).BindingContext as QuestionVM).IsSelected = false;
            (this.BindingContext as EditSurveyViewModel).ItemSelected.Execute(null);
            await (this.Resources["buttonFadeInAnimation"] as FadeToAnimation).Begin();
        }

        async void Handle_Focused(object sender, FocusEventArgs e)
        {
            await (this.Resources["buttonFadeOutAnimation"] as FadeToAnimation).Begin();
            (this.BindingContext as EditSurveyViewModel).ItemSelected.Execute(((sender as Editor).BindingContext as QuestionVM));
            //((sender as Editor).BindingContext as QuestionVM).IsSelected = true;
        }
    }
}
