using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class ReverseCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ICollection files)
            {
                return Visibility.Visible;
            }
            return files.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
