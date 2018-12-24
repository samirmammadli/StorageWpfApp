using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods
{
    public static class ToDouble
    {
        public static double StringToDouble(this string value)
        {
            double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.Float, CultureInfo.InvariantCulture, out double temp);

            return Math.Round(temp, 2);
        }
    }
}
