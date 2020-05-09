using System;
using System.Globalization;
using Xamarin.Forms;

namespace MeetupSurvey.Infrastructure
{
    public class InverseBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
