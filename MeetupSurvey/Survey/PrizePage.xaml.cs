using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Autofac;
using CarouselView.FormsPlugin.Abstractions;
using FFImageLoading.Forms;
using Lottie.Forms;
using MeetupSurvey.DTO;
using MeetupSurvey.Theming;
using ReactiveUI;
using Xamanimation;
using Xamarin.Forms;
using static MeetupSurvey.Survey.PrizeViewModel;

namespace MeetupSurvey.Survey
{
    public partial class PrizePage : ContentPage
    {
        public PrizeViewModel vm => this.BindingContext as PrizeViewModel;
        int height;
        //double AnimationHeight;
        public PrizePage(IPlatformStyling platformStyling)
        {
            InitializeComponent();
            height = platformStyling.GetStatusBarHeight();
            layout.Padding = new Thickness(0, height - 23, 0, 0);
            backgroundImage.Margin = new Thickness(0, height * -1, 0, 0);
        }

        //public void Handle_AnimationHeight(object sender, EventArgs e)
        //{
        //    AnimationView PrizeBox = (AnimationView)sender;
        //    AnimationHeight = PrizeBox.Height;
        //    imageFrame.WidthRequest = AnimationHeight * 0.18;
        //}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext != null)
            {
                (this.BindingContext as PrizeViewModel).WhenAnyValue(x => x.PageState).Subscribe(async state =>
                {
                    switch (state)
                    {
                        case PrizePageState.Loading:
                            await (this.Resources["backgroundFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["loadingFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["whiteBoxFadeInAnimation"] as FadeInAnimation).Begin();
                            await Task.Delay(700);
                            await (this.Resources["loadingFadeOutAnimation"] as FadeOutAnimation).Begin();
                            await (this.Resources["carouselFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["titleLabelFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["ellipsisFadeInAnimation"] as FadeInAnimation).Begin();
                            await (this.Resources["entriesFadeInAnimation"] as FadeInAnimation).Begin();
                            break;

                        case PrizePageState.ShakeBox:

                            await ShakeBox();
                            break;

                        case PrizePageState.ShowWinner:
                            await ShowWinner();
                            break;

                        case PrizePageState.Done:
                            break;
                    }
                })
                .DisposeWith((this.BindingContext as PrizeViewModel).DestroyWith);
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

        private async Task ShakeBox()
        {
            //if (StateList.Count > position)
            //{
            var grid = boxGrid;
            var animation = prizeBox;

            if ((!vm.SelectedPrize.ShowedWinner))
            {
                animation.Play();
                (grid.Resources["prizeBoxFadeInAnimation"] as FadeToAnimation).Begin();
            }
            else if ((vm.SelectedPrize.ShowedWinner))
            {
                animation.Progress = 1;
                (grid.Resources["prizeBoxFadeInAnimation"] as FadeToAnimation).Begin();
            }
            //}
        }

        private async Task HideWinner()
        {
            var grid = boxGrid;
            //var animation = prize.BoxAnimation;
           (grid.Resources["winnerBannerFadeOutAnimation"] as FadeToAnimation).Begin();
           (grid.Resources["winnerInformationFadeOutAnimation"] as FadeToAnimation).Begin();
           (grid.Resources["prizeBoxFadeOutAnimation"] as FadeToAnimation).Begin();
        }
        //List<PrizeState> StateList = new List<PrizeState>();
        private async Task ShowWinner()
        {
            var prize = vm.SelectedPrize;
            if (prize != null)
            {
                //if (StateList.Count > position)
                //{
                var grid = boxGrid;
                var animation = prizeBox;
                //if (animation.Opacity.Equals(0); 



                //foreach (var prize in (this.BindingContext as PrizeViewModel).Prizes)
                //prize.ShowedWinner = false;

                if (prize.WinnerName != null)
                {
                    animation.Progress = 1;
                    prize.ShowedWinner = true;
                    ShakeBox();
                    //await (grid.Resources["imageFrameFadeInAnimation"] as FadeInAnimation).Begin();
                    (grid.Resources["winnerBannerFadeInAnimation"] as FadeToAnimation).Begin();
                    (grid.Resources["winnerInformationFadeInAnimation"] as FadeToAnimation).Begin();
                }
                else if (prize.WinnerName == null)
                {
                    animation.Progress = 0;
                    animation.IsVisible = false;
                    animation.Opacity = 0;
                    animation.IsVisible = true;
                    
                    //prize.CanExecutePrizeDraw = false;
                    //prize.CanExecutePrizeDraw = true;
                }
                (grid.Resources["prizeBoxFadeInAnimation"] as FadeToAnimation).Begin();
                //}
            }
        }

        async void Handle_PositionSelected(object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e)
        {
            //if (this.BindingContext != null && (this.BindingContext as PrizeViewModel).PageState == PrizePageState.Done)
                //await ShowWinner(e.NewValue);
        }


        async void Handle_BindingContextChanged(object sender, System.EventArgs e)
        {
            //var prize = (sender as Grid).BindingContext as PrizeVM;
            ////var index = (this.BindingContext as PrizeViewModel).Prizes.IndexOf(prize);
            //if (prize != null)
            //{
            //    var state = StateList.Where(x => x.PrizeId == prize.Id).FirstOrDefault();
            //    if (state != null)
            //        StateList.Remove(state);
            //    //if (state == null)
            //    //{
            //        var boxAnimation = (sender as Grid).FindByName("prizeBox") as AnimationView;
            //        StateList.Add(new PrizeState() { PrizeId = prize.Id, Grid = (sender as Grid), BoxAnimation = boxAnimation });
            //    //}
            //    //await ShowWinner(prize.Id);
            //}
        }

        async void Handle_ItemAppearing(PanCardView.CardsView view, PanCardView.EventArgs.ItemAppearingEventArgs args)
        {
            ////var prize = args.Item as PrizeVM;
            ////var index = (this.BindingContext as PrizeViewModel).Prizes.IndexOf(prize);

            ////var state = StateList.Where(x => x.Position == index).FirstOrDefault();
            ////if (state == null)
            ////{
            ////    var boxAnimation = view.Children[index].FindByName("prizeBox") as AnimationView;
            ////    var grid = view.Children[index].FindByName("rootGrid") as Grid;
            ////    StateList.Add(new PrizeState() { PrizeId = prize.Id, Position = carousel.SelectedIndex, Grid = grid, BoxAnimation = boxAnimation });
            ////}
            //if (args.Item != null)
            //{
            //    (args.Item as PrizeVM).ShowedWinner = false;
            //    await HideWinner();
            //}
            //await Task.Delay(1000);
            //if (this.BindingContext != null && (this.BindingContext as PrizeViewModel).PageState == PrizePageState.Done)
                //await ShowWinner();
        }

        async void Handle_UserInteracted(PanCardView.CardsView view, PanCardView.EventArgs.UserInteractedEventArgs args)
        {
            if(args.Status == PanCardView.Enums.UserInteractionStatus.Started)
            {
                Console.WriteLine("Started");
                await HideWinner();
            }
            else if (args.Status == PanCardView.Enums.UserInteractionStatus.Ended)
            {
                Console.WriteLine("Ended");
                await ShowWinner();
            }
        }

        async void Handle_ItemDisappearing(PanCardView.CardsView view, PanCardView.EventArgs.ItemDisappearingEventArgs args)
        {
            //if (args.Item != null)
            //{
            //    (args.Item as PrizeVM).ShowedWinner = false;
            //    await HideWinner();
            //}


        }

        async void Handle_ItemSwiped(PanCardView.CardsView view, PanCardView.EventArgs.ItemSwipedEventArgs args)
        {
            //if (args.Item != null)
            //{
            //    (args.Item as PrizeVM).ShowedWinner = false;
            //    await HideWinner();
            //}
        }

    }


    public class PrizeState
    {
        public string PrizeId;
        //public int Position;
        public Grid Grid;
        public AnimationView BoxAnimation;
    }


}
