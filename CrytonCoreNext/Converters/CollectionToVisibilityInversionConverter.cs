using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class CollectionToVisibilityInversionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ICollection files)
            {
                return Visibility.Visible;
            }
            else if (parameter is string countThreshold)
            {
                if (!Int32.TryParse(countThreshold, out int count))
                {
                    return Visibility.Visible;
                }
                else
                {
                    if (count > 0)
                    {
                        return files.Count >= count ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            return files.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
