using CrytonCoreNext.PDF.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class PdfFormatToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EPDFFormat format)
            {
                return format == EPDFFormat.Original ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
