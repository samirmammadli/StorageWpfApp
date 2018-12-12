using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace StorageWpfApp.Converters
{
    public class QuantityToColorConverter : DependencyObject, IValueConverter
    {
        public int MinValue
        {
            get { return (int)this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
          "MinValue", typeof(int), typeof(QuantityToColorConverter), new PropertyMetadata(1));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //MessageBox.Show(((int)value).ToString() );

            return (int)value <= ConvertersStaticData.MinValue ?
                true
                : false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
