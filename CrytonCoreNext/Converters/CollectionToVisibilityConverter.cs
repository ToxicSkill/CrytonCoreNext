﻿using System;
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
            return files.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
