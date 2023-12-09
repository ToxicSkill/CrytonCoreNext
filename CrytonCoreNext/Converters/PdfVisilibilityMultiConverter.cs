using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CrytonCoreNext.Converters
{
    public class PdfVisilibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(x => (bool)x == false);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
