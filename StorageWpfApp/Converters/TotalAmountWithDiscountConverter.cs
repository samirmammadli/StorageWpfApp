using System;
using System.Globalization;
using System.Windows.Data;

namespace StorageWpfApp.Converters
{
    public class TotalAmountWithDiscountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((double)values[0] - (double)values[1]).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
