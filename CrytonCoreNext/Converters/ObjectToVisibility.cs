using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class ObjectToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val)
            {
                return string.IsNullOrEmpty(val) ? Visibility.Collapsed : Visibility.Visible;
            }
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
