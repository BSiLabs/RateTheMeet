using System;
using System.Globalization;
using Xamarin.Forms;

namespace MeetupSurvey.Infrastructure
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int opacity = 0;
            if ((bool)value == true)
                opacity = 1;

            if(parameter != null && parameter.ToString() == "invert")
            {
                if (opacity == 1)
                    opacity = 0;
                else
                    opacity = 1;
            }
            return opacity;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
