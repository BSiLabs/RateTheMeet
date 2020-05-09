using System;
using System.Linq;
using Prism.AppModel;
using Prism.Services;
using Xamarin.Forms;

namespace MeetupSurvey.Theming
{
    static public class IconFontFamily
    {
        static public string Get(string FontFamilyKey, RuntimePlatform platform)
        {
            var resources = (Application.Current.Resources[FontFamilyKey] as OnPlatform<string>).Platforms;
            switch (platform)
            {
                case RuntimePlatform.Android:
                    var android = resources.Where(x => x.Platform.FirstOrDefault() == "Android").FirstOrDefault();
                    return android.Value.ToString();
                case RuntimePlatform.iOS:
                    var iOS = resources.Where(x => x.Platform.FirstOrDefault() == "iOS").FirstOrDefault();
                    return iOS.Value.ToString();
                default:
                    return null;
            }
        }
    }
}
