using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ICollection files)
            {
                return Visibility.Collapsed;
            }
            else if (parameter is string countThreshold)
            {
                var count = 0;
                if (!Int32.TryParse(countThreshold, out count))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    if (count > 0)
                    {
                        return files.Count >= count ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
            return files.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
