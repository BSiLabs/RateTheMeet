using FFImageLoading.Forms;
using MeetupSurvey.DTO;
using MeetupSurvey.Survey;
using Xamanimation;
using Xamarin.Forms;

namespace MeetupSurvey.Theming
{
    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
            InitializeComponent();
        }

        async void Handle_Success(object sender, CachedImageEvents.SuccessEventArgs e)
        {
            ((sender as CachedImage).BindingContext as SurveyGroup).ImageLoaded = true;
            await((sender as CachedImage).Resources["imageFadeToAnimation"] as FadeToAnimation).Begin();
        }

        void Handle_Error(object sender, CachedImageEvents.ErrorEventArgs e)
        {
            ((sender as CachedImage).BindingContext as SurveyGroup).ImageLoaded = true;
            //TODO: Show default image here?
        }
    }
}
