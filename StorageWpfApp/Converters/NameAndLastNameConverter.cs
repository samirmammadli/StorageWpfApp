﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace StorageWpfApp.Converters
{
    class NameAndLastNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (values[0] as string) + " " + (values[1] as string);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
