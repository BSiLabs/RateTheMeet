using System;
using System.Globalization;
using Xamarin.Forms;

namespace MeetupSurvey.Infrastructure
{

    public class IsNullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull;
            if (value == null)
                isNull = true;
            else
                isNull = false;

            if (parameter != null && parameter.ToString() == "invert")
            {
                isNull = !isNull;
            }
            return isNull;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
