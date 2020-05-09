using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using MeetupSurvey.Theming;
using Microcharts;
using ReactiveUI;
using SkiaSharp;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Survey
{
    public partial class SurveyResultPage : ContentPage
    {
        int height;
        public SurveyResultPage(IPlatformStyling platformStyling)
        {
            InitializeComponent();
            height = platformStyling.GetStatusBarHeight();
            layout.Padding = new Thickness(0, height, 0, 0);
            backgroundImage.Margin = new Thickness(0, height * -1, 0, 0);
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext != null)
            {
                (this.BindingContext as SurveyResultViewModel).WhenAnyValue(x => x.PageState).Subscribe(async state =>
                {
                    switch (state)
                    {
                        case SurveyResultPageState.Loading:
                            await (this.Resources["backgroundFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["loadingFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["whiteBoxFadeInAnimation"] as FadeInAnimation).Begin();
                            break;

                        case SurveyResultPageState.ShowScreen:
                            await (this.Resources["carouselFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["titleLabelFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["commentLabelFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                            await (this.Resources["ellipsisFadeInAnimation"] as FadeInAnimation).Begin();
                            break;
                        case SurveyResultPageState.Done:
                            await Task.Delay(1000);
                            loading.IsVisible = false;
                            break;
                    }
                })
                .DisposeWith((this.BindingContext as SurveyResultViewModel).DestroyWith);

                (this.BindingContext as SurveyResultViewModel).WhenAnyValue(x => x.Position)
                    .Subscribe(position =>
                    {
                        if (position != startingPosition)
                        {
                            startingDirection = null;
                        }
                })
                .DisposeWith((this.BindingContext as SurveyResultViewModel).DestroyWith);
            }
        }
        protected override async void OnAppearing()
        {

            App.MasterDetail.IsGestureEnabled = false;
            carousel.Scrolled += (sender, e) =>
            {
                int position = (this.BindingContext as SurveyResultViewModel).Position;
                int max = ((this.BindingContext as SurveyResultViewModel).SurveyQuestionResults.Count * 100) - 100;

                int positionMultiplier = 0;

                if (position != 0)
                    positionMultiplier = position * 100;

                int increment = Convert.ToInt32(e.NewValue);

                //Console.WriteLine("Increment: " + increment);

                if (increment <= 1 && increment >= -1)
                    startingDirection = null;

                if (startingDirection == null && (increment >= 1 || increment >= -1))
                {
                    startingDirection = e.Direction;
                    startingPosition = position;
                }
                //Console.WriteLine("Start Direction: " + startingDirection);
                //Console.WriteLine("Current Direction: " + e.Direction);

                if ((e.Direction == CarouselView.FormsPlugin.Abstractions.ScrollDirection.Left &&
                    startingDirection == CarouselView.FormsPlugin.Abstractions.ScrollDirection.Left) ||

                (e.Direction == CarouselView.FormsPlugin.Abstractions.ScrollDirection.Right &&
                    startingDirection == CarouselView.FormsPlugin.Abstractions.ScrollDirection.Left))
                {
                    increment = increment * -1;
                    Console.WriteLine("Inverting");
                }

                int scroll = increment + (positionMultiplier);
                //Console.WriteLine("Scroll: " + scroll);
                //Console.WriteLine("");
                if (scroll == -2)
                    scroll = 0;


                if (scroll > max)
                    isOver = true;
                else
                {
                    isOver = false;
                }


                if (scroll <=2)
                    isNegative = true;
                else
                    isNegative = false;


                if(!isNegative)// && !isOver)
                    backgroundImage.Margin = new Thickness((scroll * .2) * -1, height * -1, 0, 0);

                if ((scroll - positionMultiplier <= 2 && scroll - positionMultiplier >= -2) ||
                (positionMultiplier - scroll <= 2 && positionMultiplier - scroll >= -2))
                    startingDirection = null;

            };
        }
        bool isNegative = false;
        bool isOver = false;
        int lastPosition = 0;
        int startingPosition = 0;
        CarouselView.FormsPlugin.Abstractions.ScrollDirection? startingDirection = null;
    }

}
