using System;
using System.Globalization;
using System.Windows.Data;
using CrytonCoreNext.Enums;

namespace CrytonCoreNext.Converters
{
    public class PasswordStrenghtToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return (EStrength)value  >= EStrength.Reasonable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
